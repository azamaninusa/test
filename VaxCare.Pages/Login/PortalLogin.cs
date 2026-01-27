using OpenQA.Selenium;
using Serilog;
using VaxCare.Core;
using VaxCare.Core.Entities;
using VaxCare.Core.Extensions;
using VaxCare.Core.Logger;
using VaxCare.Core.WebDriver;
using VaxCare.Pages.Portal;

namespace VaxCare.Pages.Login
{
    public class PortalLogin(IWebDriver driver, ILogger logger) : BasePage(driver, logger)
    {
        private static string IncorrectPassword { get; } = "IncorrectPassword";
        private static string User { get; set; } = "";
        private static string Password { get; set; } = "";

        // TODO: Add production check for url. Maybe allow certain users to run in production?
        private const string _userNameFieldId = "idp-discovery-username";
        private const string _nextButtonId = "idp-discovery-submit";
        private const string _passwordFieldId = "okta-signin-password";
        private const string _signInButtonId = "okta-signin-submit";

        // TODO: Figure out if the Driver and all page tools make it to InitAsync or if they need to be explicitely passed
        public override async Task InitAsync(object[] args)
        {
            await SetUser(args[0]);
        }

        /// <summary>
        /// Async login method for the Portal
        /// </summary>
        /// <returns> The Portal page after authentication</returns>
        public async Task<PortalPage> LoginAsync(string url, bool failLogin = false)
        {
            var passwordToUse = failLogin ? IncorrectPassword : Password;
            Log.Step("Login to the Portal.");
            await Driver.NavigateAsync(url);
            await Driver.FindElementAsync(_userNameFieldId.Id());
            await Driver.SendKeysAsync(_userNameFieldId.Id(), User);
            await Driver.ClickAsync(_nextButtonId.Id());
            await Driver.SendKeysAsync(_passwordFieldId.Id(), passwordToUse, true);
            await Driver.ClickAsync(_signInButtonId.Id());
            return await GoToAsync(async _ => new PortalPage(Driver, Log));
        }

        private static Task SetUser(object args)
        {
            var user = args as User;
            if (user != null)
            {
                User = user.Username;
                Password = user.Password;
            }
            else
                throw new ArgumentException("Invalid argument for Login routine");

            return Task.CompletedTask;
        }
    }
}
