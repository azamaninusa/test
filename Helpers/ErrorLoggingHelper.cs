using System.Diagnostics;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using Serilog;

namespace VaxCare.Core.Helpers
{
    /// <summary>
    /// Helper class for consistent error logging with file paths, URLs, and method names
    /// </summary>
    public static class ErrorLoggingHelper
    {
        /// <summary>
        /// Logs error details including method name, file location, current URL, and exception details
        /// </summary>
        public static void LogErrorWithContext(
            ILogger logger,
            Exception ex,
            string errorMessage,
            IWebDriver? driver = null)
        {
            // Get current URL if driver is available
            string currentUrl = GetCurrentUrl(driver);

            // Get method name and file info from stack trace
            var (methodName, fileName, lineNumber) = GetStackTraceInfo(ex);

            // Format file path as clickable URL
            string clickableFileUrl = FormatFileUrl(fileName, lineNumber);

            // Log all error details
            logger.Error(ex, errorMessage);
            logger.Error($"Failed Method: {methodName}");
            logger.Error($"Failed Location: {clickableFileUrl} (line {lineNumber})");
            logger.Error($"Current URL: {currentUrl}");
            logger.Error($"Exception Type: {ex.GetType().Name}");
            logger.Error($"Exception Message: {ex.Message}");

            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                logger.Error($"Stack Trace:\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Gets the current URL from the driver
        /// </summary>
        private static string GetCurrentUrl(IWebDriver? driver)
        {
            try
            {
                if (driver != null)
                {
                    return driver.Url;
                }
            }
            catch
            {
                // Ignore if URL can't be retrieved
            }
            return "N/A";
        }

        /// <summary>
        /// Extracts method name, file name, and line number from exception stack trace
        /// Looks for the actual method in the fluent chain, not just wrapper methods
        /// </summary>
        private static (string methodName, string fileName, int lineNumber) GetStackTraceInfo(Exception ex)
        {
            // Check if exception message contains chain step info
            if (ex.Message.Contains("Failed in chain step:"))
            {
                var stepInfo = ex.Message.Replace("Failed in chain step: ", "").Split('\n')[0].Trim();
                if (!string.IsNullOrEmpty(stepInfo))
                {
                    // Try to get file info from inner exception
                    var innerEx = ex.InnerException;
                    if (innerEx != null)
                    {
                        var innerStackTrace = new StackTrace(innerEx, true);
                        for (int i = 0; i < innerStackTrace.FrameCount; i++)
                        {
                            var frame = innerStackTrace.GetFrame(i);
                            if (frame?.GetMethod()?.DeclaringType?.Name?.EndsWith("Page") == true)
                            {
                                return (stepInfo, frame.GetFileName() ?? "Unknown", frame.GetFileLineNumber());
                            }
                        }
                    }
                    return (stepInfo, "Unknown", 0);
                }
            }

            var stackTrace = new StackTrace(ex, true);
            
            string methodName = "Unknown";
            string fileName = "Unknown";
            int lineNumber = 0;

            // Look through stack frames to find the actual method that failed
            // Skip wrapper methods like "Then", "RunTestAsync", etc.
            var skipPatterns = new[] { "Then", "RunTestAsync", "MoveNext", "Invoke", "ExecuteAsync", "<>c", "AsyncStateMachine" };
            
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                var frame = stackTrace.GetFrame(i);
                if (frame == null) continue;

                var method = frame.GetMethod();
                if (method == null) continue;

                var declaringType = method.DeclaringType;
                if (declaringType == null) continue;

                var methodNameOnly = method.Name;
                var typeName = declaringType.Name;
                var fullMethodName = $"{typeName}.{methodNameOnly}";

                // Skip compiler-generated and wrapper methods
                bool shouldSkip = skipPatterns.Any(pattern => 
                    methodNameOnly.Contains(pattern) || 
                    typeName.Contains(pattern));

                if (shouldSkip)
                {
                    continue;
                }

                // Found a real method - check if it's from a page object or test
                // Look for methods ending in "Async" or classes ending in "Page"
                if (methodNameOnly.EndsWith("Async") || 
                    typeName.EndsWith("Page") || 
                    typeName.EndsWith("Test"))
                {
                    methodName = fullMethodName;
                    fileName = frame.GetFileName() ?? "Unknown";
                    lineNumber = frame.GetFileLineNumber();
                    
                    // Prefer methods from Page classes
                    if (typeName.EndsWith("Page"))
                    {
                        break;
                    }
                }
                // If this is the first non-skipped frame, use it as fallback
                else if (methodName == "Unknown")
                {
                    methodName = fullMethodName;
                    fileName = frame.GetFileName() ?? "Unknown";
                    lineNumber = frame.GetFileLineNumber();
                }
            }

            return (methodName, fileName, lineNumber);
        }

        /// <summary>
        /// Formats file path as clickable file:// URL with line number anchor
        /// </summary>
        private static string FormatFileUrl(string fileName, int lineNumber)
        {
            if (fileName == "Unknown" || string.IsNullOrEmpty(fileName))
            {
                return "N/A";
            }

            try
            {
                var fullPath = Path.GetFullPath(fileName).Replace("\\", "/");
                if (fullPath.Contains(":"))
                {
                    return $"file:///{fullPath.Replace(":", "")}#{lineNumber}";
                }
                else
                {
                    return $"file://{fullPath}#{lineNumber}";
                }
            }
            catch
            {
                return fileName;
            }
        }
    }
}
