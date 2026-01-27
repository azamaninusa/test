using OpenQA.Selenium;

namespace VaxCare.Core.WebDriver
{
    /// <summary>
    /// Interface that wraps IWebDriver to provide a clean abstraction for page objects.
    /// Extension methods on IWebDriver can be called directly on IWebDriverActor instances.
    /// </summary>
    public interface IWebDriverActor
    {
        IWebDriver Driver { get; }
        string Url => Driver.Url;
    }

    /// <summary>
    /// Implementation of IWebDriverActor that wraps an IWebDriver instance.
    /// This allows page objects to use IWebDriverActor while extension methods work on the underlying IWebDriver.
    /// </summary>
    public class WebDriverActor : IWebDriverActor
    {
        public IWebDriver Driver { get; }

        public WebDriverActor(IWebDriver driver)
        {
            Driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }
    }
}
