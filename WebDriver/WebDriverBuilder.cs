using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace VaxCare.Core.WebDriver
{
    public enum BrowserType
    {
        Chrome, Firefox, Edge
    }

    public interface IWebDriverBuilder
    {
        IWebDriver Build();
        WebDriverBuilder WithArguments();
        WebDriverBuilder WithBrowser();
    }

    public class WebDriverBuilder : IWebDriverBuilder
    {
        private readonly WebDriverSettings _settings;
        private IWebDriver _driver;
        private DriverOptions _driverOptions;
        private const string headless = "--headless";

        public WebDriverBuilder(IOptions<WebDriverSettings> settings)
        {
            _settings = settings.Value;
        }

        public WebDriverBuilder WithBrowser()
        {
            switch (_settings.BrowserType)
            {
                case BrowserType.Chrome:
                    _driverOptions = new ChromeOptions();
                    break;
                case BrowserType.Firefox:
                    _driverOptions = new FirefoxOptions();
                    break;
                case BrowserType.Edge:
                    _driverOptions = new EdgeOptions();
                    break;
                default:
                    throw new ArgumentException("Unsupported browser type");
            }
            return this;
        }

        public WebDriverBuilder WithArguments()
        {
            if (_settings.Headless)
            {
                if (_driverOptions is ChromeOptions chromeOptions)
                {
                    chromeOptions.AddArgument(headless);
                    // Docker-specific arguments for running Chrome in containers
                    chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArgument("--disable-dev-shm-usage");
                    chromeOptions.AddArgument("--disable-gpu");
                    chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.Severe);
                }

                if (_driverOptions is FirefoxOptions firefoxOptions)
                {
                    firefoxOptions.AddArgument(headless);
                    firefoxOptions.SetLoggingPreference(LogType.Browser, LogLevel.Severe);
                }

                if (_driverOptions is EdgeOptions edgeOptions)
                {
                    edgeOptions.AddArgument(headless);
                    edgeOptions.AddArgument("--no-sandbox");
                    edgeOptions.AddArgument("--disable-dev-shm-usage");
                    edgeOptions.AddArgument("--disable-gpu");
                    edgeOptions.SetLoggingPreference(LogType.Browser, LogLevel.Severe);
                }
            }
            return this;
        }

        public IWebDriver Build()
        {
            if (_driverOptions is ChromeOptions chromeOptions)
                _driver = new ChromeDriver(chromeOptions);

            if (_driverOptions is FirefoxOptions firefoxOptions)
                _driver = new FirefoxDriver(firefoxOptions);

            if (_driverOptions is EdgeOptions edgeOptions)
                _driver = new EdgeDriver(edgeOptions);

            return _driver;
        }
    }
}
