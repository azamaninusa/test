using OpenQA.Selenium;
using Serilog;
using VaxCare.Core.WebDriver;

namespace VaxCare.Core
{
    public abstract class BasePage
    {
        protected IWebDriver Driver { get; }
        protected ILogger Log { get; }

        protected BasePage(IWebDriver driver, ILogger logger)
        {
            Driver = driver;
            Log = logger;
        }

        /// <summary>
        /// Allows for page initialization before the test begins
        /// Override this if page requires preloading, waiting, login, or navigation
        /// </summary>
        /// <returns></returns>
        public virtual Task InitAsync(params object[] args) => Task.CompletedTask;

        /// <summary>
        /// Fluent chaining.
        /// </summary>
        /// <returns>Initializes and returns the next page instance</returns>
        protected async Task<T> GoToAsync<T>(T page, params object[] args) where T : BasePage
        {
            await page.InitAsync(args);
            return page;
        }

        /// <summary>
        /// Fluent chaining.
        /// </summary>
        /// <returns>Auto-constructs and initializes a new page</returns>
        protected async Task<T> GoToAsync<T>(params object[] args) where T : BasePage
        {
            var page = (T)Activator.CreateInstance(typeof(T), Driver, Log)!;
            await page.InitAsync(args);
            return page;
        }
    }
}