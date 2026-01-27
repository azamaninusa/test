using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;

namespace VaxCare.Core.WebDriver
{
    /// <summary>
    /// Extension methods for IWebDriverActor that delegate to the underlying IWebDriver extension methods
    /// </summary>
    public static class WebDriverActorExtensions
    {
        public static async Task ClickAsync(this IWebDriverActor actor, By by, int timeoutInSeconds = 10)
            => await actor.Driver.ClickAsync(by, timeoutInSeconds);

        public static async Task ClickAsync(this IWebDriverActor actor, IWebElement element)
            => await actor.Driver.ClickAsync(element);

        public static async Task SendKeysAsync(this IWebDriverActor actor, By by, string text, int timeoutInSeconds = 10)
            => await actor.Driver.SendKeysAsync(by, text, timeoutInSeconds);

        public static async Task SendKeysAsync(this IWebDriverActor actor, IWebElement element, string text)
            => await actor.Driver.SendKeysAsync(element, text);

        public static async Task<IWebElement> FindElementAsync(this IWebDriverActor actor, By by, int timeoutInSeconds = 10)
            => await actor.Driver.FindElementAsync(by, timeoutInSeconds);

        public static async Task<List<IWebElement>> FindAllElementsAsync(this IWebDriverActor actor, By parentSelector, By childSelector, int timeoutInSeconds = 10)
            => await actor.Driver.FindAllElementsAsync(parentSelector, childSelector, timeoutInSeconds);

        public static async Task<List<IWebElement>> FindAllChildElementsAsync(this IWebDriverActor actor, IWebElement parent, By childSelector, int timeoutInSeconds = 10)
            => await actor.Driver.FindAllChildElementsAsync(parent, childSelector, timeoutInSeconds);

        public static async Task<string> GetTextAsync(this IWebDriverActor actor, By by, int timeoutInSeconds = 10)
            => await actor.Driver.GetTextAsync(by, timeoutInSeconds);

        public static async Task<bool> ElementExistsAsync(this IWebDriverActor actor, By by)
            => await actor.Driver.ElementExistsAsync(by);

        public static async Task<bool> IsElementPresentAsync(this IWebDriverActor actor, By by, int timeoutInSeconds = 10)
            => await actor.Driver.IsElementPresentAsync(by, timeoutInSeconds);

        public static async Task<bool> IsTextPresentAsync(this IWebDriverActor actor, By by, string text, int timeoutInSeconds = 10)
            => await actor.Driver.IsTextPresentAsync(by, text, timeoutInSeconds);

        public static async Task NavigateAsync(this IWebDriverActor actor, string url)
            => await actor.Driver.NavigateAsync(url);

        public static async Task SelectDropDownOptionByValueAsync(this IWebDriverActor actor, By by, string optionToSelect, int timeoutInSeconds = 10)
            => await actor.Driver.SelectDropDownOptionByValueAsync(by, optionToSelect, timeoutInSeconds);

        public static async Task SelectDropDownOptionByTextAsync(this IWebDriverActor actor, By by, string optionText, int timeoutInSeconds = 10)
        {
            var dropdown = await actor.Driver.FindElementAsync(by, timeoutInSeconds);
            var selectElement = new SelectElement(dropdown);
            selectElement.SelectByText(optionText);
        }

        public static async Task WaitForAlertAndDismissAsync(this IWebDriverActor actor, int timeoutInSeconds = 10)
            => await actor.Driver.WaitForAlertAndDismissAsync(timeoutInSeconds);

        public static async Task WaitForElementToDisappearAsync(this IWebDriverActor actor, By by, int timeoutInSeconds = 10)
            => await actor.Driver.WaitForElementToDisappearAsync(by, timeoutInSeconds);

        public static async Task WaitUntilElementLoadsAsync(this IWebDriverActor actor, By by, int timeoutInSeconds = 10)
            => await actor.Driver.WaitUntilElementLoadsAsync(by, timeoutInSeconds);

        public static async Task<bool> IsElementClickableAsync(this IWebDriverActor actor, By by, int timeoutInSeconds = 10)
            => await actor.Driver.IsElementClickableAsync(by, timeoutInSeconds);

        public static async Task ExecuteJavaScriptClickAsync(this IWebDriverActor actor, By by, int timeoutInSeconds = 10)
            => await actor.Driver.ExecuteJavaScriptClickAsync(by, timeoutInSeconds);

        public static async Task<string> GetCssValueAsync(this IWebDriverActor actor, IWebElement element, string propertyName)
            => await actor.Driver.GetCssValueAsync(element, propertyName);

        public static async Task CheckBrowserConsoleErrorsAsync(this IWebDriverActor actor, ILogger logger)
            => await actor.Driver.CheckBrowserConsoleErrorsAsync(logger);
    }
}
