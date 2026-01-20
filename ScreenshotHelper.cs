using OpenQA.Selenium;

namespace VaxCare.Core.Helpers
{
    public static class ScreenshotHelper
    {
        public static string CaptureScreenshot(IWebDriver driver, string testName)
        {
            try
            {
                // Check if driver is null
                if (driver == null)
                {
                    Console.WriteLine("Failed to capture screenshot: Driver is null");
                    return string.Empty;
                }

                // Check if driver supports screenshots
                if (!(driver is ITakesScreenshot))
                {
                    Console.WriteLine($"Failed to capture screenshot: Driver does not implement ITakesScreenshot. Driver type: {driver.GetType().Name}");
                    return string.Empty;
                }

                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                
                // Use TestResults directory for screenshots (works in Docker and local)
                // Always use absolute paths to avoid path mixing issues between Windows and Linux
                string screenshotsDir;
                var currentDir = Directory.GetCurrentDirectory();
                Console.WriteLine($"Current working directory: {currentDir}");
                
                // Try Docker Linux path first (most common in CI/CD)
                // Check if we're in Docker by looking for /app directory
                var dockerLinuxPath = "/app/TestResults";
                if (Directory.Exists("/app") && (Directory.Exists(dockerLinuxPath) || currentDir.StartsWith("/app")))
                {
                    screenshotsDir = "/app/TestResults/screenshots";
                    Console.WriteLine($"Using Docker Linux path: {screenshotsDir}");
                }
                // Try Docker Windows path
                else if (Directory.Exists("C:\\app\\TestResults"))
                {
                    screenshotsDir = "C:\\app\\TestResults\\screenshots";
                    Console.WriteLine($"Using Docker Windows path: {screenshotsDir}");
                }
                // Try relative TestResults path (normalize to absolute)
                else
                {
                    // Build absolute path manually to avoid Path.Combine issues with mixed separators
                    // Normalize all backslashes to forward slashes for Linux compatibility
                    var normalizedCurrentDir = currentDir.Replace('\\', '/').TrimEnd('/');
                    
                    // Check if TestResults exists relative to current dir
                    if (Directory.Exists(Path.Combine(currentDir, "TestResults")))
                    {
                        screenshotsDir = $"{normalizedCurrentDir}/TestResults/screenshots";
                        Console.WriteLine($"Using relative TestResults path: {screenshotsDir}");
                    }
                    else
                    {
                        // Create TestResults in current directory
                        screenshotsDir = $"{normalizedCurrentDir}/TestResults/screenshots";
                        Console.WriteLine($"Using fallback path (will create): {screenshotsDir}");
                    }
                }
                
                // Ensure path doesn't contain Windows drive letters or mixed separators
                screenshotsDir = screenshotsDir.Replace('\\', '/');
                if (screenshotsDir.Contains(":/") && !screenshotsDir.StartsWith("/"))
                {
                    // If path contains Windows drive (C:/), extract just the path part
                    var parts = screenshotsDir.Split(new[] { ":/" }, StringSplitOptions.None);
                    if (parts.Length > 1)
                    {
                        screenshotsDir = "/" + parts[1].TrimStart('/');
                        Console.WriteLine($"Cleaned Windows path, using: {screenshotsDir}");
                    }
                }
                
                Console.WriteLine($"Final screenshots directory: {screenshotsDir}");
                
                // Create directory if it doesn't exist
                try
                {
                    Directory.CreateDirectory(screenshotsDir);
                    Console.WriteLine($"Screenshots directory created/verified: {screenshotsDir}");
                }
                catch (Exception dirEx)
                {
                    Console.WriteLine($"Failed to create directory {screenshotsDir}: {dirEx.Message}");
                    throw;
                }
                
                // Sanitize test name for filename (remove invalid characters)
                var sanitizedTestName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var filename = $"{sanitizedTestName}_{timestamp}.png";
                
                // Build filepath manually to avoid Path.Combine issues with mixed path separators
                // Ensure we use the correct separator for the screenshotsDir path
                var filepath = screenshotsDir.TrimEnd('/', '\\') + "/" + filename;
                
                Console.WriteLine($"Attempting to save screenshot to: {filepath}");
                Console.WriteLine($"Directory exists: {Directory.Exists(screenshotsDir)}");
                
                screenshot.SaveAsFile(filepath);
                Console.WriteLine($"Screenshot successfully saved to: {filepath}");
                return filepath;
            }
            catch (Exception ex)
            {
                // Log error but don't throw - screenshot failure shouldn't break the test
                Console.WriteLine($"Failed to capture screenshot: {ex.Message}");
                Console.WriteLine($"Screenshot error stack trace: {ex.StackTrace}");
                return string.Empty;
            }
        }
    }
}
