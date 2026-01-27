using OpenQA.Selenium;
using Serilog;
using VaxCare.Core;
using VaxCare.Core.Extensions;
using VaxCare.Core.Logger;
using VaxCare.Core.WebDriver;
using VaxCare.Pages.DataEntry;

namespace VaxCare.Pages.Login
{
    public class DataEntryLogin(IWebDriver driver, ILogger log) : BasePage(driver, log)
    {
        private static readonly string UserNameFieldId = "i0116";
        private static readonly string NextButtonId = "idSIButton9";
        private static readonly string PasswordFieldId = "input28";
        private static readonly string OktaVerifyButton = "//input[@value='Verify']";
        private static readonly string DoNotStaySignedIn = "idBtn_Back";
        private static readonly string UserName = "qauser@vaxcare.com";
        private static readonly string Password = "Samm1chesCheeseBac.n";

        public async Task<DataEntryHomePage> LoginAsync(string url)
        {
            Log.Step("Login to Data Entry.");
            await Driver.NavigateAsync(url);
            await Driver.SendKeysAsync(UserNameFieldId.Id(), UserName);
            await Driver.ClickAsync(NextButtonId.Id());
            await Driver.SendKeysAsync(PasswordFieldId.Id(), Password, true);
            await Driver.ClickAsync(OktaVerifyButton.XPath());
            await Driver.ClickAsync(DoNotStaySignedIn.Id());
            return await GoToAsync(async _ => new DataEntryHomePage(Driver, Log));
        }
    }
}
