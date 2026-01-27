using OpenQA.Selenium;
using Serilog;

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
        
        Task<T> ExecuteAsync<T>(Func<IWebDriver, Task<T>> action);
        Task ExecuteAsync(Func<IWebDriver, Task> action);
    }

    /// <summary>
    /// Implementation of IWebDriverActor that wraps an IWebDriver instance.
    /// This allows page objects to use IWebDriverActor while extension methods work on the underlying IWebDriver.
    /// </summary>
    public class WebDriverActor : IWebDriverActor
    {
        public IWebDriver Driver { get; }
        internal ILogger? _logger;

        public WebDriverActor(IWebDriver driver, ILogger? logger = null)
        {
            Driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _logger = logger;
        }

        public async Task<T> ExecuteAsync<T>(Func<IWebDriver, Task<T>> action)
        {
            return await action(Driver);
        }

        public async Task ExecuteAsync(Func<IWebDriver, Task> action)
        {
            await action(Driver);
        }
    }
}
