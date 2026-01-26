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

        public static WebDriverActionBuilder Actions(this IWebDriver driver)
        {
            return new WebDriverActionBuilder(driver);
        }

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
        /// <returns>The IWebElement found or a WebDriverTimeoutException will bubble up to indicate error.</returns>
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

        public static async Task<IWebElement> FindElementAsync(this IWebDriver driver, By by, ILogger logger, int timeoutInSeconds = 10)
        {
            return await driver.Actions()
                               .WithLogger(logger)
                               .ExecuteAsync(drv => drv.FindElementAsync(by, timeoutInSeconds), $"Find Element: {by}");
        }

        /// <summary>
        /// Finds the parent element, then finds all child elements and returns them in a list.
        /// </summary>
        /// <returns>A list of IWebElements found under the Parent element or a WebDriverTimeoutException will bubble up to indicate error.</returns>
        public static async Task<List<IWebElement>> FindAllElementsAsync(this IWebDriver driver, By parentSelector, By childSelector, int timeoutInSeconds = 10)
        {
            var parent = await driver.FindElementAsync(parentSelector, timeoutInSeconds);
            return parent.FindElements(childSelector).ToList();
        }

        public static async Task<List<IWebElement>> FindAllElementsAsync(this IWebDriver driver, By parentSelector, By childSelector, ILogger logger, int timeoutInSeconds = 10)
        {
            return await driver.Actions()
                               .WithLogger(logger)
                               .ExecuteAsync(drv => drv.FindAllElementsAsync(parentSelector, childSelector, timeoutInSeconds), $"Find all child Elements of: {parentSelector}");
        }

        /// <summary>
        /// Finds all elements matching a single selector.
        /// </summary>
        /// <returns>A list of IWebElements matching the selector.</returns>
        public static async Task<List<IWebElement>> FindAllElementsBySelectorAsync(this IWebDriver driver, By selector, int timeoutInSeconds = 10)
        {
            return await Task.Run(() =>
            {
                var wait = GetWait(driver, timeoutInSeconds);
                return wait.Until(drv =>
                {
                    var elements = drv.FindElements(selector);
                    return elements.Count > 0 ? elements.ToList() : null;
                }) ?? new List<IWebElement>();
            });
        }

        public static async Task<List<IWebElement>> FindAllElementsBySelectorAsync(this IWebDriver driver, By selector, ILogger logger, int timeoutInSeconds = 10)
        {
            return await driver.Actions()
                               .WithLogger(logger)
                               .ExecuteAsync(drv => drv.FindAllElementsBySelectorAsync(selector, timeoutInSeconds), $"Find all Elements matching: {selector}");
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

        public static async Task<List<IWebElement>> FindAllChildElementsAsync(this IWebDriver driver, IWebElement parent, By childSelector, ILogger logger, int timeoutInSeconds = 10)
        {
            return await driver.Actions()
                               .WithLogger(logger)
                               .ExecuteAsync(drv =>
                               drv.FindAllChildElementsAsync(parent, childSelector, timeoutInSeconds), $"Find all child Elements of WebElement: {parent}");
        }

        /// <summary>
        /// Waits and clicks on an element retrieved via a selector after it finishes loading.
        /// </summary>
        /// <returns>No return. If the element is not clickable a WebDriverTimeoutException will bubble up to indicate error</returns>
        public static async Task ClickAsync(this IWebDriver driver, By by, int timeoutInSeconds = 10)
        {
            var element = await driver.FindElementAsync(by, timeoutInSeconds);
            element.Click();
        }

        public static async Task ClickAsync(this IWebDriver driver, By by, ILogger logger, int timeoutInSeconds = 10)
        {
            await driver.Actions()
                        .WithLogger(logger)
                        .ExecuteAsync(drv => drv.ClickAsync(by), $"Click: {by}");
        }

        /// <summary>
        /// Clicks on the given element, without waiting for it.
        /// </summary>
        /// <returns></returns>
        public static async Task ClickAsync(this IWebDriver driver, IWebElement element)
        {
            await Task.Run(() => element.Click());
        }

        public static async Task ClickAsync(this IWebDriver driver, IWebElement element, ILogger logger)
        {
            await driver.Actions()
                        .WithLogger(logger)
                        .ExecuteAsync(drv => drv.ClickAsync(element), $"Click WebElement: {element}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static async Task<string> GetTextAsync(this IWebDriver driver, By by, int timeoutInSeconds = 10)
        {
            var element = await driver.FindElementAsync(by, timeoutInSeconds);
            return element.Text;
        }

        public static async Task<string> GetTextAsync(this IWebDriver driver, By by, ILogger logger, int timeoutInSeconds = 10)
        {
            return await driver.Actions()
                               .WithLogger(logger)
                               .ExecuteAsync(drv => drv.GetTextAsync(by, timeoutInSeconds), $"Get Text from Selector: {by}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        public static async Task<bool> ElementExistsAsync(this IWebDriver driver, By by, ILogger logger)
        {
            return await driver.Actions()
                               .WithLogger(logger)
                               .ExecuteAsync(drv => drv.ElementExistsAsync(by), $"Check if Element Exists: {by}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        public static async Task<bool> IsElementClickableAsync(this IWebDriver driver, By by, ILogger logger, int timeoutInSeconds = 10)
        {
            return await driver.Actions()
                               .WithLogger(logger)
                               .ExecuteAsync(drv => drv.IsElementClickableAsync(by), $"Check if Element is Clickable: {by}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        public static async Task<bool> IsElementPresentAsync(this IWebDriver driver, By by, ILogger logger, int timeoutInSeconds = 10)
        {
            return await driver.Actions()
                               .WithLogger(logger)
                               .ExecuteAsync(drv => drv.IsElementPresentAsync(by, timeoutInSeconds), $"Check if Element is Present: {by}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsTextPresentAsync(this IWebDriver driver, By by, string text, int timeoutInSeconds = 10)
        {
            var element = await driver.FindElementAsync(by, timeoutInSeconds);
            return element.Text.Contains(text);
        }

        public static async Task<bool> IsTextPresentAsync(this IWebDriver driver, By by, string text, ILogger logger, int timeoutInSeconds = 10)
        {
            return await driver.Actions()
                               .WithLogger(logger)
                               .ExecuteAsync(drv => drv.IsTextPresentAsync(by, text, timeoutInSeconds), $"Check if Text '{text}' Exists in: {by}");
        }

        public static async Task NavigateAsync(this IWebDriver driver, string url, ILogger logger)
        {
            await Task.Run(() =>
            {
                logger.Information($"Navigating to {url}");
                driver.Navigate().GoToUrl(url);
            });
            
            // Wait for page to be in a ready state to prevent blank screenshots
            await Task.Run(async () =>
            {
                var jsExecutor = (IJavaScriptExecutor)driver;
                var maxWaitTime = TimeSpan.FromSeconds(10);
                var startTime = DateTime.Now;
                
                while (DateTime.Now - startTime < maxWaitTime)
                {
                    try
                    {
                        var readyState = jsExecutor.ExecuteScript("return document.readyState");
                        if (readyState?.ToString() == "complete")
                        {
                            // Additional small delay to ensure rendering is complete
                            await Task.Delay(500);
                            logger.Information("Page load complete");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warning($"Error checking page ready state: {ex.Message}");
                    }
                    
                    await Task.Delay(100);
                }
                
                logger.Warning("Page load timeout - proceeding anyway");
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
        /// Selects a specific option in a drop-down by visible text.
        /// </summary>
        public static async Task SelectDropDownOptionByTextAsync(this IWebDriver driver, By by, string optionToSelect, int timeoutInSeconds = 10)
        {
            var dropdown = await driver.FindElementAsync(by, timeoutInSeconds);
            var selectElement = new SelectElement(dropdown);
            selectElement.SelectByText(optionToSelect);
        }

        public static async Task SelectDropDownOptionByTextAsync(this IWebDriver driver, By by, string optionToSelect, ILogger logger, int timeoutInSeconds = 10)
        {
            await driver.Actions()
                       .WithLogger(logger)
                       .ExecuteAsync(drv => drv.SelectDropDownOptionByTextAsync(by, optionToSelect, timeoutInSeconds), $"Select option '{optionToSelect}' from dropdown: {by}");
        }

        /// <summary>
        /// Gets the CSS value of an element.
        /// </summary>
        public static async Task<string> GetCssValueAsync(this IWebDriver driver, IWebElement element, string propertyName, ILogger logger)
        {
            return await Task.Run(() =>
            {
                var value = element.GetCssValue(propertyName);
                logger.Information($"CSS property '{propertyName}' value: {value}");
                return value;
            });
        }

        public static async Task SelectDropDownOptionByValueAsync(this IWebDriver driver, By by, string optionToSelect, ILogger logger, int timeoutInSeconds = 10)
        {
            await driver.Actions()
                        .WithLogger(logger)
                        .ExecuteAsync(drv =>
                        drv.SelectDropDownOptionByValueAsync(by, optionToSelect, timeoutInSeconds), $"Select Drop-down Option: {optionToSelect}");
        }

        /// <summary>
        /// Test
        /// </summary>
        /// <returns>Nothing</returns>
        public static async Task SendKeysAsync(this IWebDriver driver, By by, string text, int timeoutInSeconds = 10)
        {
            var element = await driver.FindElementAsync(by, timeoutInSeconds);

            if (!string.IsNullOrEmpty(element.Text))
                element.Clear();

            element.SendKeys(text);
        }

        public static async Task SendKeysAsync(this IWebDriver driver, By by, string text, ILogger logger, bool isPassword = false, int timeoutInSeconds = 10)
        {
            var logText = text;
            if (isPassword)
                logText = new string('*', text.Length);

            await driver.Actions()
                        .WithLogger(logger)
                        .ExecuteAsync(drv => drv.SendKeysAsync(by, text, timeoutInSeconds), $"Send Text '{logText}' to Selector : {by}");
        }

        /// <summary>
        /// 
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

        public static async Task SendKeysAsync(this IWebDriver driver, IWebElement element, string text, ILogger logger, bool isPassword = false)
        {
            var logText = text;
            if (isPassword)
                logText = new string('*', text.Length);

            await driver.Actions()
                        .WithLogger(logger)
                        .ExecuteAsync(drv => drv.SendKeysAsync(element, text), $"Send Text '{logText}' to WebDriver Element : {element}");
        }

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
        /// 
        /// </summary>
        /// <returns></returns>
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

        public static async Task WaitForElementToDisappearAsync(this IWebDriver driver, By by, ILogger logger, int timeoutInSeconds = 10)
        {
            await driver.Actions()
                        .WithLogger(logger)
                        .ExecuteAsync(drv =>
                        drv.WaitForElementToDisappearAsync(by, timeoutInSeconds), $"Wait up to {timeoutInSeconds} for Element {by} to Disapppear");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        public static async Task WaitForElementToDisappearAsync(this IWebDriver driver, IWebElement element, ILogger logger, int timeoutInSeconds = 10)
        {
            await driver.Actions()
                        .WithLogger(logger)
                        .ExecuteAsync(drv =>
                        drv.WaitForElementToDisappearAsync(element, timeoutInSeconds), $"Wait up to {timeoutInSeconds} for Element {element} to Disapppear");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        public static async Task WaitUntilElementLoadsAsync(this IWebDriver driver, By by, int timeoutInSeconds = 10)
        {
            await driver.FindElementAsync(by, timeoutInSeconds);
        }

        public static async Task WaitUntilElementLoadsAsync(this IWebDriver driver, By by, ILogger logger, int timeoutInSeconds = 10)
        {
            await driver.Actions()
                        .WithLogger(logger)
                        .ExecuteAsync(drv => drv.WaitUntilElementLoadsAsync(by, timeoutInSeconds), $"Wait up to {timeoutInSeconds}s Until {by} Loads");
        }
    }
}
