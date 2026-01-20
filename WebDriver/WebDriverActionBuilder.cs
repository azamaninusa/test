using OpenQA.Selenium;
using Serilog;

namespace VaxCare.Core.WebDriver
{
    public class WebDriverActionBuilder
    {
        private readonly IWebDriver _driver;
        private ILogger? _logger;

        public WebDriverActionBuilder(IWebDriver driver)
        {
            _driver = driver;
        }

        public WebDriverActionBuilder WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        public async Task<T> ExecuteAsync<T>(Func<IWebDriver, Task<T>> action, string? description = null)
        {
            if (_logger != null && !string.IsNullOrEmpty(description))
                _logger.Information(description);
            return await action(_driver);
        }

        public async Task ExecuteAsync(Func<IWebDriver, Task> action, string? description = null)
        {
            if (_logger != null && !string.IsNullOrEmpty(description))
                _logger.Information(description);
            await action(_driver);
        }
    }
}
