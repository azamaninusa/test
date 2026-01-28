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

        // Test Runner wrapper
        protected async Task RunTestAsync(string testName, Func<Task> testBody)
        {
            try
            {
                _currentTestName = testName;
                Log.Information($"Starting Test: {testName}");
                await testBody();
                await HandleTestSuccessAsync(testName);
            }
            catch (Exception ex)
            {
                await HandleTestFailureAsync(ex, testName, null);
                throw;
            }
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
                var clickablePath = screenshotPath.Replace('\\', '/');
                if (!clickablePath.StartsWith("file://"))
                {
                    // Convert Windows path to file:// URL format
                    if (Path.IsPathRooted(screenshotPath))
                    {
                        clickablePath = "file:///" + clickablePath.Replace(":", "").Replace("\\", "/");
                    }
                    else
                    {
                        var fullPath = Path.GetFullPath(screenshotPath).Replace("\\", "/").Replace(":", "");
                        clickablePath = "file:///" + fullPath;
                    }
                }
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
