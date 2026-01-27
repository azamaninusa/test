using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;

namespace VaxCare.Core.WebDriver
{
    public static class WebDriverExtensions
    {

        private static WebDriverWait GetWait(IWebDriver driver, int timeoutInSeconds)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds))
            {
                PollingInterval = TimeSpan.FromMilliseconds(500)
            };
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(ElementNotInteractableException),
                typeof(StaleElementReferenceException),
                typeof(WebDriverTimeoutException)
                );

            return wait;
        }

        /// <summary>
        /// This method checks if there are Console Errors on the browser and logs the issues.
        /// </summary>
        /// <returns></returns>
        public static async Task CheckBrowserConsoleErrorsAsync(this IWebDriver driver, ILogger logger)
        {
            await Task.Run(() =>
            {
                var logEntries = driver.Manage().Logs.GetLog(LogType.Browser);
                var errorLogs = logEntries.Where(x => x.Level == LogLevel.Severe).ToList();
                if (errorLogs.Any())
                {
                    foreach (var logEntry in errorLogs)
                    {
                        logger.Error($"Browser console error: {logEntry.Message}");
                    }
                    throw new Exception("Brower console errors were found. See logs for details");
                }
            });

        }

        /// <summary>
        /// Find elements via a selector. This method waits for the element to load before attempting to find it.
        /// </summary>
        /// <returns> The IWebElement found or a WebDriverTimeoutException will bubble up to indicate error.</returns>
        public static async Task<IWebElement> FindElementAsync(this IWebDriver driver, By by, int timeoutInSeconds = 10)
        {
            var wait = GetWait(driver, timeoutInSeconds);
            var element = await Task.Run(() =>
            {
                return wait.Until(drv =>
                {
                    var el = drv.FindElement(by);
                    return el != null && el.Displayed && el.Enabled ? el : null;
                });
            });
            return element;
        }

        /// <summary>
        /// Finds the parent element, then finds all child elements and returns them in a list.
        /// </summary>
        /// <returns> A list of IWebElements found under the Parent element or a WebDriverTimeoutException will bubble up to indicate error.</returns>
        public static async Task<List<IWebElement>> FindAllElementsAsync(this IWebDriver driver, By parentSelector, By childSelector, int timeoutInSeconds = 10)
        {
            var parent = await driver.FindElementAsync(parentSelector, timeoutInSeconds);
            return parent.FindElements(childSelector).ToList();
        }


        /// <summary>
        /// Finds and returns a list of all child Elements for a given Parent element. Includes Logging and Retry on Exception.
        /// </summary>
        /// <returns>A list of IWebElements found under the Parent element or a WebDriverTimeoutException will bubble up to indicate error.</returns>
        public static async Task<List<IWebElement>> FindAllChildElementsAsync(this IWebDriver driver, IWebElement parent, By childSelector, int timeoutInSeconds = 10)
        {
            return await Task.Run(() =>
            {
                return parent.FindElements(childSelector).ToList();
            });
        }

        /// <summary>
        /// Waits and clicks on an element retrieved via a selector after it finishes loading.
        /// </summary>
        /// <returns> No return. If the element is not clickable a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task ClickAsync(this IWebDriver driver, By by, int timeoutInSeconds = 10)
        {
            var element = await driver.FindElementAsync(by, timeoutInSeconds);
            element.Click();
        }

        /// <summary>
        /// Clicks on the given element, without waiting for it.
        /// </summary>
        /// <returns></returns>
        public static async Task ClickAsync(this IWebDriver driver, IWebElement element)
        {
            await Task.Run(() => element.Click());
        }

        /// <summary>
        /// Retrieves the text of an element via Selectors
        /// </summary>
        /// <returns>A string containing the element's text if it is found or a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task<string> GetTextAsync(this IWebDriver driver, By by, int timeoutInSeconds = 10)
        {
            var element = await driver.FindElementAsync(by, timeoutInSeconds);
            return element.Text;
        }

        /// <summary>
        /// Checks if an element exists on the page.
        /// </summary>
        /// <returns> True or False if the element exists. </returns>
        public static async Task<bool> ElementExistsAsync(this IWebDriver driver, By by)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _ = driver.FindElement(by);
                    return Task.FromResult(true);
                }
                catch (NoSuchElementException)
                {
                    return Task.FromResult(false);
                }
                catch (ArgumentNullException)
                {
                    return Task.FromResult(false);
                }
            });
        }

        /// <summary>
        /// Checks if an element on the page is clickable.
        /// </summary>
        /// <returns> True or False if the element can be clicked. </returns>
        public static async Task<bool> IsElementClickableAsync(this IWebDriver driver, By by, int timeoutInSeconds = 10)
        {
            try
            {
                var element = await driver.FindElementAsync(by, timeoutInSeconds);
                return element.Displayed && element.Enabled;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
            catch (ElementNotInteractableException)
            {
                return false;
            }

        }

        /// <summary>
        /// Checks if the Element found via a Selector exists on the page. This method will wait for the element to show up.
        /// </summary>
        /// <returns> True or False if the element Exists. </returns>
        public static async Task<bool> IsElementPresentAsync(this IWebDriver driver, By by, int timeoutInSeconds = 10)
        {
            try
            {
                var element = await driver.FindElementAsync(by, timeoutInSeconds);
                return element != null && element.Displayed && element.Enabled; ;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the Element for the given Selector contains the expected Text,
        /// </summary>
        /// <returns> True or False if the Text matches. </returns>
        public static async Task<bool> IsTextPresentAsync(this IWebDriver driver, By by, string text, int timeoutInSeconds = 10)
        {
            var element = await driver.FindElementAsync(by, timeoutInSeconds);
            return element.Text.Contains(text);
        }

        /// <summary>
        /// This method navigates to the given Url
        /// </summary>
        /// <returns></returns>
        public static async Task NavigateAsync(this IWebDriver driver, string url)
        {
            await Task.Run(() =>
            {
                driver.Navigate().GoToUrl(url);
            });
        }

        /// <summary>
        /// Selects a specific option in a drop-down.
        /// </summary>
        /// <returns></returns>
        public static async Task SelectDropDownOptionByValueAsync(this IWebDriver driver, By by, string optionToSelect, int timeoutInSeconds = 10)
        {
            var dropdown = await driver.FindElementAsync(by, timeoutInSeconds);
            var selectElement = new SelectElement(dropdown); // Unable to unit test due to this line.
            selectElement.SelectByValue(optionToSelect);
        }

        /// <summary>
        /// Sends keys to an Element via a given Selector.
        /// </summary>
        /// <returns></returns>
        public static async Task SendKeysAsync(this IWebDriver driver, By by, string text, int timeoutInSeconds = 10)
        {
            var element = await driver.FindElementAsync(by, timeoutInSeconds);

            if (!string.IsNullOrEmpty(element.Text))
                element.Clear();

            element.SendKeys(text);
        }

        /// <summary>
        /// Sends keys to an Element.
        /// </summary>
        /// <returns></returns>
        public static async Task SendKeysAsync(this IWebDriver driver, IWebElement element, string text)
        {
            await Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(element.Text))
                    element.Clear();
                element.SendKeys(text);
            });
        }

        /// <summary>
        /// Waits until an Alert is fired and Dismisses it.
        /// </summary>
        /// <returns> No return. If the Alert does not appear within the given time, a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task WaitForAlertAndDismissAsync(this IWebDriver driver, int timeoutInSeconds = 10)
        {
            var wait = GetWait(driver, timeoutInSeconds);
            wait.IgnoreExceptionTypes(typeof(NoAlertPresentException));

            await Task.Run(() =>
            {
                wait.Until(drv =>
                {
                    return drv.SwitchTo().Alert();
                });
                string currentWindowHandle = driver.CurrentWindowHandle;
                driver.SwitchTo().Alert().Dismiss();
                driver.SwitchTo().Window(currentWindowHandle);
            });
        }

        /// <summary>
        /// Waits until an Element found via a given Selector disappears from the Page.
        /// </summary>
        /// <returns> No return. If the Element does not disappear within the given time, a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task WaitForElementToDisappearAsync(this IWebDriver driver, By by, int timeoutInSeconds = 10)
        {
            var wait = GetWait(driver, timeoutInSeconds);
            await Task.Run(() =>
            {
                wait.Until(drv =>
                {
                    try
                    {
                        var element = drv.FindElement(by);
                        return !element.Displayed;
                    }
                    catch (NoSuchElementException)
                    {
                        return true;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return true;
                    }
                });
            });
        }

        /// <summary>
        /// Waits until an Element disappears from the Page.
        /// </summary>
        /// <returns> No return. If the Element does not disappear within the given time, a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task WaitForElementToDisappearAsync(this IWebDriver driver, IWebElement element, int timeoutInSeconds = 10)
        {
            var wait = GetWait(driver, timeoutInSeconds);
            await Task.Run(() =>
            {
                wait.Until(drv =>
                {
                    try
                    {
                        return !element.Displayed;
                    }
                    catch (NoSuchElementException)
                    {
                        return true;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return true;
                    }
                });
            });
        }

        /// <summary>
        /// Waits until an Element found via a given Selector loads on the Page.
        /// </summary>
        /// <returns> No return. If the Element does not show up within the given time, a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task WaitUntilElementLoadsAsync(this IWebDriver driver, By by, int timeoutInSeconds = 10)
        {
            await driver.FindElementAsync(by, timeoutInSeconds);
        }
    }
}