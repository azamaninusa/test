using System.Diagnostics;
using System.IO;
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
        /// </summary>
        private static (string methodName, string fileName, int lineNumber) GetStackTraceInfo(Exception ex)
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
