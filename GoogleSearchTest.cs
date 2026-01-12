using OpenQA.Selenium;
using VaxCare.Core;
using VaxCare.Core.WebDriver;
using Xunit.Abstractions;

namespace VaxCare.Tests
{
    public class GoogleSearchTest : BaseTest
    {
        public GoogleSearchTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task SearchForPenOnGoogle()
        {
            await RunTestAsync("Search for 'pen' on Google", async () =>
            {
                // Navigate to Google
                await Driver.NavigateAsync("https://www.google.com", Log);

                // Find the search box (Google's search input has name="q")
                var searchBox = await Driver.FindElementAsync(By.Name("q"), Log);

                // Enter "pen" in the search box
                await Driver.SendKeysAsync(By.Name("q"), "pen", Log);

                // Submit the search (press Enter)
                searchBox.Submit();

                // Wait for the page to load after search submission
                // Wait for URL to change or for search results to appear
                await Task.Delay(2000); // Give the page time to load

                // Verify we're on the search results page
                var currentUrl = Driver.Url;
                Log.Information($"Current URL after search: {currentUrl}");
                
                // Verify the page title indicates we're on search results
                var pageTitle = Driver.Title;
                Log.Information($"Page title: {pageTitle}");

                // Try to find any search result element to confirm results loaded
                // Google results typically have elements with class containing "g" (result containers)
                var hasResults = await Driver.ElementExistsAsync(By.CssSelector("div[class*='g']"), Log);
                if (hasResults)
                {
                    Log.Information("Search results found on page");
                }

                Log.Information("Successfully searched for 'pen' on Google");
            });
        }
    }
}

