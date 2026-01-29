using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using Serilog;
using VaxCare.Core.Helpers;
using VaxCare.Core.Logger;
using VaxCare.Core.WebDriver;
using Xunit;
using Xunit.Abstractions;

namespace VaxCare.Core
{
    public abstract class BaseTest : IAsyncLifetime
    {
        protected IServiceProvider Services;
        private IServiceScope _scope;
        private IWebDriverBuilder _driverBuilder;
        private ILoggerBuilder _loggerBuilder;
        protected readonly ITestOutputHelper _output;
        private readonly string _teamsWebhook = "Teams webhook URL";
        private string? _currentTestName;
        protected IWebDriver Driver { get; private set; }
        protected ILogger Log { get; private set; }

        public BaseTest(ITestOutputHelper output)
        {
            _output = output;
            var startup = new TestStartup();
            _scope = startup.Services.CreateScope();
            Services = _scope.ServiceProvider;
        }

        public async Task InitializeAsync()
        {
            _driverBuilder = Services.GetRequiredService<IWebDriverBuilder>();
            _loggerBuilder = Services.GetRequiredService<ILoggerBuilder>();
            Log = _loggerBuilder.WithOutput(_output).WithConfiguration().Build();

            if (Driver != null)
            {
                Log.Warning("Driver was not null at test start. Starting up a new driver");
                Driver.Quit();
            }

            Driver = _driverBuilder.WithBrowser().WithArguments().Build();
            Log.Information("Webdriver created");
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            if (Driver != null)
            {
                Driver.Quit();
                Driver.Dispose();
                Log.Information("Webdriver closed");
            }
            _scope.Dispose();
            Log.Information($"Test '{_currentTestName}' finished");
            await Task.CompletedTask;
        }

        // Base page factory 
        // TODO Refactor and remove constructorArgs
        protected T Page<T>(params object[] args) where T : BasePage
        {
            var webDriverActor = new WebDriverActor(Driver, Log);
            var constructorArgs = new object[] { webDriverActor, Log }.Concat(args).ToArray();
            return (T)Activator.CreateInstance(typeof(T), webDriverActor, Log)!;
        }

        // Allows for a new page to be instantiated allowing for the page to be initialized first!
        protected async Task<T> PageAsync<T>(params object[] args) where T : BasePage
        {
            var page = Page<T>(args);
            await page.InitAsync(args);
            return page;
        }

        // Test Runner wrapper with retry logic (up to 3 attempts).
        // On retry: kill the browser, start a new session, then run the failed test from the beginning.
        protected async Task RunTestAsync(string testName, Func<Task> testBody)
        {
            const int maxRetries = 3;
            Exception? lastException = null;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    _currentTestName = testName;

                    if (attempt == 1)
                    {
                        Log.Information($"Starting Test: {testName}");
                    }
                    else
                    {
                        // Retry: kill browser and start a new session, then run test from the beginning
                        Log.Warning($"Retrying Test: {testName} (Attempt {attempt}/{maxRetries})");
                        KillBrowserAndCreateNewSession();
                    }

                    await testBody();
                    await HandleTestSuccessAsync(testName);
                    return; // Test succeeded, exit retry loop
                }
                catch (Exception ex)
                {
                    lastException = ex;

                    if (attempt < maxRetries)
                    {
                        Log.Warning($"Test failed on attempt {attempt}/{maxRetries}. Will kill browser and retry from beginning.");
                        Log.Warning($"Exception: {ex.Message}");
                        // Brief delay before next attempt (browser will be killed and recreated at start of next attempt)
                        await Task.Delay(1000 * attempt);
                    }
                    else
                    {
                        // Final attempt failed - log and throw
                        Log.Error($"Test failed after {maxRetries} attempts.");
                        await HandleTestFailureAsync(ex, testName, $"Failed after {maxRetries} attempts");
                        throw;
                    }
                }
            }

            // This should never be reached, but just in case
            if (lastException != null)
            {
                throw lastException;
            }
        }

        /// <summary>
        /// Quits and disposes the current browser/driver, then creates a new session.
        /// Used when retrying a failed test so the retry runs from a clean state.
        /// </summary>
        private void KillBrowserAndCreateNewSession()
        {
            if (Driver != null)
            {
                try
                {
                    Driver.Quit();
                }
                catch (Exception ex)
                {
                    Log.Warning($"Driver.Quit() threw: {ex.Message}");
                }

                try
                {
                    Driver.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Warning($"Driver.Dispose() threw: {ex.Message}");
                }

                Driver = null;
                Log.Information("Browser session closed.");
            }

            Driver = _driverBuilder.WithBrowser().WithArguments().Build();
            Log.Information("New browser session started for retry.");
        }

        // Success and Failure handlers
        protected async Task HandleTestFailureAsync(Exception ex, string testName, string? contextInfo = null)
        {
            // Log error with context using helper (this already logs the error message)
            ErrorLoggingHelper.LogErrorWithContext(Log, ex, $"Test failed: {testName}", Driver);
            
            // Capture screenshot on failure
            var screenshotPath = ScreenshotHelper.CaptureScreenshot(Driver, testName);
            if (!string.IsNullOrEmpty(screenshotPath))
            {
                // Format path as clickable file:// URL for easy navigation in IDEs
                // Use the same formatting logic as ErrorLoggingHelper.FormatFileUrl
                string clickablePath = FormatScreenshotPath(screenshotPath);
                Log.Error($"Screenshot saved to: {clickablePath}");
                Log.Information($"Screenshot path (raw): {screenshotPath}");
            }
            else
            {
                Log.Warning("Failed to capture screenshot");
            }

            // Get error details for Teams message
            var (methodName, fileName, lineNumber) = GetErrorDetails(ex);
            string currentUrl = GetCurrentUrl(Driver);

            // Message to teams
            var message = $"**Test Failed: '{testName}'**\n" +
                $"Method: {methodName}\n" +
                $"Location: {fileName}:{lineNumber}\n" +
                $"URL: {currentUrl}\n" +
                $"Exception: {ex.Message}\n";
            if (!string.IsNullOrEmpty(contextInfo))
                message += $"Context: {contextInfo}\n";
            if (!string.IsNullOrEmpty(screenshotPath))
                message += $"Screenshot: {screenshotPath}";

            await TeamsNotifierHelper.SendMessageAsync(_teamsWebhook, message);
        }

        private static (string methodName, string fileName, int lineNumber) GetErrorDetails(Exception ex)
        {
            var stackTrace = new StackTrace(ex, true);
            var frame = stackTrace.GetFrame(0);
            
            string methodName = "Unknown";
            string fileName = "Unknown";
            int lineNumber = 0;
            
            if (frame != null)
            {
                var method = frame.GetMethod();
                methodName = method != null ? $"{method.DeclaringType?.Name}.{method.Name}" : "Unknown";
                fileName = frame.GetFileName() ?? "Unknown";
                lineNumber = frame.GetFileLineNumber();
            }
            
            return (methodName, fileName, lineNumber);
        }

        private static string GetCurrentUrl(IWebDriver? driver)
        {
            try
            {
                if (driver != null)
                {
                    return driver.Url;
                }
            }
            catch { }
            return "N/A";
        }

        /// <summary>
        /// Formats screenshot path as clickable file:// URL
        /// Handles both Windows (C:\path) and Unix (/path) paths
        /// </summary>
        private static string FormatScreenshotPath(string screenshotPath)
        {
            if (string.IsNullOrEmpty(screenshotPath))
            {
                return "N/A";
            }

            try
            {
                // Normalize the path - convert backslashes to forward slashes
                var normalizedPath = screenshotPath.Replace("\\", "/");
                
                // Handle Windows absolute paths (e.g., C:\path\to\file.png)
                if (normalizedPath.Length >= 3 && 
                    char.IsLetter(normalizedPath[0]) && 
                    normalizedPath[1] == ':' && 
                    normalizedPath[2] == '/')
                {
                    // Windows path: C:/path/to/file.png -> file:///C:/path/to/file.png
                    return $"file:///{normalizedPath}";
                }
                // Handle Unix absolute paths (e.g., /path/to/file.png)
                else if (normalizedPath.StartsWith("/"))
                {
                    // Unix path: /path/to/file.png -> file:///path/to/file.png
                    return $"file://{normalizedPath}";
                }
                // Handle relative paths - convert to absolute
                else
                {
                    var fullPath = Path.GetFullPath(screenshotPath).Replace("\\", "/");
                    if (fullPath.Length >= 3 && 
                        char.IsLetter(fullPath[0]) && 
                        fullPath[1] == ':' && 
                        fullPath[2] == '/')
                    {
                        return $"file:///{fullPath}";
                    }
                    else if (fullPath.StartsWith("/"))
                    {
                        return $"file://{fullPath}";
                    }
                    else
                    {
                        return $"file:///{fullPath}";
                    }
                }
            }
            catch
            {
                return screenshotPath;
            }
        }

        protected async Task HandleTestSuccessAsync(string testName)
        {
            var message = $"**Test Succeeded: '{testName}'**";

            // Log the success
            Log.Information(message);

            // Message to teams
            await TeamsNotifierHelper.SendMessageAsync(_teamsWebhook, message);
        }
    }
}
