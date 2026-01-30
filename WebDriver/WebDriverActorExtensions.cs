using OpenQA.Selenium;
using Serilog;

namespace VaxCare.Core.WebDriver
{
    /// <summary>
    /// Extension methods for IWebDriverActor that mirror WebDriverExtensions
    /// All WebDriver operations go through the Actor for thread safety
    /// </summary>
    public static class WebDriverActorExtensions
    {
        // ===== ELEMENT FINDING =====

        /// <summary>
        /// Find elements via a selector. This method waits for the element to load before attempting to find it.
        /// </summary>
        /// <returns> The IWebElement found or a WebDriverTimeoutException will bubble up to indicate error.</returns>
        public static async Task<IWebElement> FindElementAsync(
            this IWebDriverActor actor, By by, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Find Element: {by}");

            return await actor.ExecuteAsync(driver =>
                driver.FindElementAsync(by, timeoutInSeconds));
        }

        /// <summary>
        /// Finds the parent element, then finds all child elements and returns them in a list.
        /// </summary>
        /// <returns> A list of IWebElements found under the Parent element or a WebDriverTimeoutException will bubble up to indicate error.</returns>
        public static async Task<List<IWebElement>> FindAllElementsAsync(
            this IWebDriverActor actor, By parentSelector, By childSelector, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Find all child Elements of: {parentSelector}");

            return await actor.ExecuteAsync(driver =>
                driver.FindAllElementsAsync(parentSelector, childSelector, timeoutInSeconds));
        }

        /// <summary>
        /// Finds and returns a list of all child Elements for a given Parent element. Includes Logging and Retry on Exception.
        /// </summary>
        /// <returns> A list of IWebElements found under the Parent element or a WebDriverTimeoutException will bubble up to indicate error.</returns>
        public static async Task<List<IWebElement>> FindAllChildElementsAsync(
            this IWebDriverActor actor, IWebElement parent, By childSelector, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Find all child Elements of WebElement: {parent}");

            return await actor.ExecuteAsync(driver =>
                driver.FindAllChildElementsAsync(parent, childSelector, timeoutInSeconds));
        }


        // ===== CLICKING =====

        /// <summary>
        /// Waits and clicks on an element retrieved via a selector after it finishes loading.
        /// </summary>
        /// <returns> No return. If the element is not clickable a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task ClickAsync(
            this IWebDriverActor actor, By by, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Click: {by}");

            await actor.ExecuteAsync(driver =>
                driver.ClickAsync(by, timeoutInSeconds));
        }

        /// <summary>
        /// Clicks on the given element, without waiting for it.
        /// </summary>
        /// <returns> No return. If the element is not clickable a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task ClickAsync(
            this IWebDriverActor actor, IWebElement element, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Click: {element}");

            await actor.ExecuteAsync(driver =>
                driver.ClickAsync(element));
        }

        // ===== TEXT OPERATIONS =====

        /// <summary>
        /// Retrieves the text of an element via Selectors
        /// </summary>
        /// <returns> A string containing the element's text if it is found or a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task<string> GetTextAsync(
            this IWebDriverActor actor, By by, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Get Text from Selector: {by}");

            return await actor.ExecuteAsync(driver =>
                driver.GetTextAsync(by, timeoutInSeconds));
        }

        /// <summary>
        /// Sends keys to an Element via a given Selector.
        /// </summary>
        /// <returns></returns>
        public static async Task SendKeysAsync(
            this IWebDriverActor actor, By by, string text, bool isPassword = false, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            var logText = text;
            if (isPassword)
                logText = new string('*', text.Length);

            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Send Text '{logText}' to Selector : {by}");

            await actor.ExecuteAsync(driver =>
                driver.SendKeysAsync(by, text, timeoutInSeconds));

            // Small delay to prevent flakiness when interacting with a page.
            // TODO: Reduce this delay as much as possible without making tests brittle
            await Task.Delay(500);
        }

        /// <summary>
        /// Sends keys to an Element.
        /// </summary>
        /// <returns></returns>
        public static async Task SendKeysAsync(
            this IWebDriverActor actor, IWebElement element, string text, bool shouldLog = true, bool isPassword = false)
        {
            var logText = text;
            if (isPassword)
                logText = new string('*', text.Length);

            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Send Text '{logText}' to WebDriver Element : {element}");

            await actor.ExecuteAsync(driver =>
                driver.SendKeysAsync(element, text));
        }

        // ===== ELEMENT STATE CHECKS =====

        /// <summary>
        /// Checks if an element exists on the page.
        /// </summary>
        /// <returns> True or False if the element exists. </returns>
        public static async Task<bool> ElementExistsAsync(
            this IWebDriverActor actor, By by, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Check if Element Exists: {by}");

            return await actor.ExecuteAsync(driver =>
                driver.ElementExistsAsync(by));
        }

        /// <summary>
        /// Checks if an element on the page is clickable.
        /// </summary>
        /// <returns> True or False if the element can be clicked. </returns>
        public static async Task<bool> IsElementClickableAsync(
            this IWebDriverActor actor, By by, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Check if Element is Clickable: {by}");

            return await actor.ExecuteAsync(driver =>
                driver.IsElementClickableAsync(by, timeoutInSeconds));
        }

        /// <summary>
        /// Checks if the Element found via a Selector exists on the page
        /// </summary>
        /// <returns> True or False if the element Exists. </returns>
        public static async Task<bool> IsElementPresentAsync(
            this IWebDriverActor actor, By by, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Check if Element is Present: {by}");

            return await actor.ExecuteAsync(driver =>
                driver.IsElementPresentAsync(by, timeoutInSeconds));
        }

        /// <summary>
        /// Verifies that each of the given XPath selectors finds at least one element on the page (DOM existence only).
        /// Matches legacy VerifyElementsPresentOnPage: uses FindElements-style check, so hidden/disabled elements still pass.
        /// </summary>
        /// <param name="actor">The web driver actor.</param>
        /// <param name="timeoutInSeconds">Seconds to poll for each element (optional). If 0, checks immediately like legacy.</param>
        /// <param name="elemXpaths">One or more XPath strings to verify.</param>
        /// <exception cref="Exception">Thrown when any element could not be found on the page.</exception>
        public static async Task VerifyElementsPresentOnPageAsync(
            this IWebDriverActor actor, int timeoutInSeconds, params string[] elemXpaths)
        {
            foreach (var xpath in elemXpaths)
            {
                var by = By.XPath(xpath);
                var found = timeoutInSeconds <= 0
                    ? await actor.ElementExistsAsync(by)
                    : await PollUntilExistsAsync(actor, by, timeoutInSeconds);
                if (!found)
                    throw new Exception($"The element could not be found on page: {xpath}");
            }
        }

        /// <summary>
        /// Verifies that each of the given XPath selectors finds at least one element on the page (DOM existence only).
        /// Immediate check like legacy VerifyElementsPresentOnPage - hidden/disabled elements count as present.
        /// </summary>
        public static async Task VerifyElementsPresentOnPageAsync(
            this IWebDriverActor actor, params string[] elemXpaths)
        {
            foreach (var xpath in elemXpaths)
            {
                var by = By.XPath(xpath);
                var present = await actor.ElementExistsAsync(by);
                if (!present)
                    throw new Exception($"The element could not be found on page: {xpath}");
            }
        }

        private static async Task<bool> PollUntilExistsAsync(IWebDriverActor actor, By by, int timeoutInSeconds)
        {
            var deadline = DateTime.UtcNow.AddSeconds(timeoutInSeconds);
            while (DateTime.UtcNow < deadline)
            {
                if (await actor.ElementExistsAsync(by))
                    return true;
                await Task.Delay(300);
            }
            return await actor.ElementExistsAsync(by);
        }

        /// <summary>
        /// Checks if the Element for the given Selector contains the expected Text,
        /// </summary>
        /// <returns> True or False if the Text matches. </returns>
        public static async Task<bool> IsTextPresentAsync(
            this IWebDriverActor actor, By by, string text, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Check if Text '{text}' Exists in: {by}");

            return await actor.ExecuteAsync(driver =>
                driver.IsTextPresentAsync(by, text, timeoutInSeconds));
        }

        // ===== NAVIGATION =====

        /// <summary>
        /// This method navigates to the given Url
        /// </summary>
        /// <returns></returns>
        public static async Task NavigateAsync(
            this IWebDriverActor actor, string url, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Navigating to {url}");

            await actor.ExecuteAsync(driver =>
                driver.NavigateAsync(url));
        }

        // ===== DROP-DOWN SELECTION =====

        /// <summary>
        /// Selects a specific option in a drop-down.
        /// </summary>
        /// <returns></returns>
        public static async Task SelectDropDownOptionByValueAsync(
            this IWebDriverActor actor, By by, string optionToSelect, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Select Drop-down Option: {optionToSelect}");

            await actor.ExecuteAsync(driver =>
                driver.SelectDropDownOptionByValueAsync(by, optionToSelect, timeoutInSeconds));
        }

        /// <summary>
        /// Selects a specific option in a drop-down by text.
        /// </summary>
        /// <returns></returns>
        public static async Task SelectDropDownOptionByTextAsync(
            this IWebDriverActor actor, By by, string optionText, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Select Drop-down Option by Text: {optionText}");

            await actor.ExecuteAsync(async driver =>
                await driver.SelectDropDownOptionByTextAsync(by, optionText, timeoutInSeconds));
        }

        // ===== WAITING =====

        /// <summary>
        /// Waits until an Element found via a given Selector loads on the Page.
        /// </summary>
        /// <returns> No return. If the Element does not show up within the given time, a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task WaitUntilElementLoadsAsync(
            this IWebDriverActor actor, By by, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Wait up to {timeoutInSeconds}s Until {by} Loads");

            await actor.ExecuteAsync(driver =>
                driver.WaitUntilElementLoadsAsync(by, timeoutInSeconds));
        }

        /// <summary>
        /// Waits until an Element found via a given Selector disappears from the Page.
        /// </summary>
        /// <returns> No return. If the Element does not disappear within the given time, a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task WaitForElementToDisappearAsync(
            this IWebDriverActor actor, By by, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Wait up to {timeoutInSeconds} for Element {by} to Disapppear");

            await actor.ExecuteAsync(driver =>
                driver.WaitForElementToDisappearAsync(by, timeoutInSeconds));
        }

        /// <summary>
        /// Waits until an Element disappears from the Page.
        /// </summary>
        /// <returns> No return. If the Element does not disappear within the given time, a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task WaitForElementToDisappearAsync(
            this IWebDriverActor actor, IWebElement element, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Wait up to {timeoutInSeconds} for Element {element} to Disapppear");

            await actor.ExecuteAsync(driver =>
                driver.WaitForElementToDisappearAsync(element, timeoutInSeconds));
        }

        /// <summary>
        /// Waits until an Alert is fired and Dismisses it.
        /// </summary>
        /// <returns> No return. If the Alert does not appear within the given time, a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task WaitForAlertAndDismissAsync(
            this IWebDriverActor actor, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Wait up to {timeoutInSeconds} for Alert and then Dismiss it.");

            await actor.ExecuteAsync(driver =>
                driver.WaitForAlertAndDismissAsync(timeoutInSeconds));
        }

        // ===== JAVASCRIPT OPERATIONS =====

        /// <summary>
        /// Clicks an element using JavaScript execution. Useful when element is intercepted by another element.
        /// </summary>
        /// <returns></returns>
        public static async Task ExecuteJavaScriptClickAsync(
            this IWebDriverActor actor, By by, int timeoutInSeconds = 10, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Execute JavaScript Click: {by}");

            await actor.ExecuteAsync(driver =>
                driver.ExecuteJavaScriptClickAsync(by, timeoutInSeconds));
        }

        /// <summary>
        /// Gets the CSS value of an element property.
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetCssValueAsync(
            this IWebDriverActor actor, IWebElement element, string propertyName, bool shouldLog = true)
        {
            if (shouldLog && actor._logger != null)
                actor._logger.Information($"Get CSS Value '{propertyName}' from Element: {element}");

            return await actor.ExecuteAsync(driver =>
                driver.GetCssValueAsync(element, propertyName));
        }

        // ===== BROWSER CONSOLE =====

        /// <summary>
        /// This method checks if there are Console Errors on the browser and logs the issues.
        /// </summary>
        /// <returns></returns>
        public static async Task CheckBrowserConsoleErrorsAsync(
            this IWebDriverActor actor, ILogger logger)
        {
            await actor.ExecuteAsync(driver =>
                driver.CheckBrowserConsoleErrorsAsync(logger));
        }
    }
}
