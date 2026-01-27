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
                
                // Determine the base directory for TestResults
                string baseDir;
                
                // Try Docker Linux path first (most common in CI/CD)
                if (Directory.Exists("/app") || currentDir.StartsWith("/app"))
                {
                    baseDir = "/app";
                    screenshotsDir = Path.Combine(baseDir, "TestResults", "screenshots");
                    Console.WriteLine($"Using Docker Linux path: {screenshotsDir}");
                }
                // Try Docker Windows path
                else if (Directory.Exists("C:\\app"))
                {
                    baseDir = "C:\\app";
                    screenshotsDir = Path.Combine(baseDir, "TestResults", "screenshots");
                    Console.WriteLine($"Using Docker Windows path: {screenshotsDir}");
                }
                // Use current directory (local development)
                else
                {
                    baseDir = currentDir;
                    screenshotsDir = Path.Combine(baseDir, "TestResults", "screenshots");
                    Console.WriteLine($"Using local path: {screenshotsDir}");
                }
                
                // Ensure directory exists - create all parent directories if needed
                try
                {
                    if (!Directory.Exists(screenshotsDir))
                    {
                        Directory.CreateDirectory(screenshotsDir);
                        Console.WriteLine($"Created screenshots directory: {screenshotsDir}");
                    }
                    else
                    {
                        Console.WriteLine($"Screenshots directory already exists: {screenshotsDir}");
                    }
                    
                    // Verify we can write to the directory
                    var testFile = Path.Combine(screenshotsDir, ".test");
                    File.WriteAllText(testFile, "test");
                    File.Delete(testFile);
                    Console.WriteLine($"Verified write access to: {screenshotsDir}");
                }
                catch (Exception dirEx)
                {
                    Console.WriteLine($"Failed to create/verify directory {screenshotsDir}: {dirEx.Message}");
                    Console.WriteLine($"Exception type: {dirEx.GetType().Name}");
                    Console.WriteLine($"Stack trace: {dirEx.StackTrace}");
                    throw;
                }
                
                // Sanitize test name for filename (remove invalid characters and spaces)
                // Replace spaces and special characters with underscores for web-friendly filenames
                var sanitizedTestName = testName
                    .Replace(" ", "_")  // Replace spaces with underscores
                    .Replace("'", "")   // Remove apostrophes
                    .Replace("\"", "")  // Remove quotes
                    .Replace(":", "_")  // Replace colons
                    .Replace("/", "_")  // Replace slashes
                    .Replace("\\", "_") // Replace backslashes
                    .Replace("?", "")   // Remove question marks
                    .Replace("*", "")   // Remove asterisks
                    .Replace("<", "")   // Remove less than
                    .Replace(">", "")   // Remove greater than
                    .Replace("|", "_"); // Replace pipe with underscore
                
                // Also remove any remaining invalid filename characters
                var invalidChars = Path.GetInvalidFileNameChars();
                foreach (var c in invalidChars)
                {
                    sanitizedTestName = sanitizedTestName.Replace(c, '_');
                }
                
                // Remove multiple consecutive underscores
                while (sanitizedTestName.Contains("__"))
                {
                    sanitizedTestName = sanitizedTestName.Replace("__", "_");
                }
                
                // Trim underscores from start and end
                sanitizedTestName = sanitizedTestName.Trim('_');
                
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var filename = $"{sanitizedTestName}_{timestamp}.png";
                
                // Build filepath using Path.Combine for proper OS-specific path handling
                var filepath = Path.Combine(screenshotsDir, filename);
                
                // Double-check directory exists before saving
                var directoryPath = Path.GetDirectoryName(filepath);
                if (string.IsNullOrEmpty(directoryPath))
                {
                    throw new InvalidOperationException($"Could not determine directory path for: {filepath}");
                }
                
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    Console.WriteLine($"Created directory before save: {directoryPath}");
                }
                
                Console.WriteLine($"Attempting to save screenshot to: {filepath}");
                Console.WriteLine($"Directory exists: {Directory.Exists(directoryPath)}");
                Console.WriteLine($"Directory is writable: {Directory.Exists(directoryPath)}");
                
                // Save screenshot
                screenshot.SaveAsFile(filepath);
                
                // Verify file was created
                if (!File.Exists(filepath))
                {
                    throw new InvalidOperationException($"Screenshot file was not created at: {filepath}");
                }
                
                var fileInfo = new FileInfo(filepath);
                Console.WriteLine($"Screenshot successfully saved to: {filepath}");
                Console.WriteLine($"File size: {fileInfo.Length} bytes");
                Console.WriteLine($"File exists: {File.Exists(filepath)}");
                
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
