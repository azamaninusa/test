using OpenQA.Selenium;
using Serilog;
using VaxCare.Core;
using VaxCare.Core.Extensions;
using VaxCare.Core.Logger;
using VaxCare.Core.WebDriver;

namespace VaxCare.Pages.DataEntry
{
    public class InsuranceMappingPage(IWebDriverActor driver, ILogger log) : BasePage(driver, log)
    {
        private const string HomePageLogoId = "lnkConsentBatchIndex";
        private const string DropdownState = "//*[@id=\"MappingFallouts\"]/div[1]/table/tr[2]/td[2]/select";
        private const string InputFilterInsuranceName = "//*[@id=\"MappingFallouts\"]/div[1]/table/tr[2]/td[1]/input";
        private const string InputFilterInsuranceName2 = "//*[@id=\"Insurances\"]/div[1]/table/tr[2]/td[2]/input";
        private const string TableSelectInsuranceToMap = "//*[@id=\"MappingFallouts\"]/div[2]/table/tbody/tr[1]/td[1]";
        private const string TableSelectInsuranceToMapInsuranceId = "//*[@id=\"MappingFallouts\"]/div[2]/table/tbody/tr[1]/td[3]";
        private const string TableSelectInsuranceToMapMappingFilterId = "//*[@id=\"MappingFallouts\"]/div[2]/table/tbody/tr[1]/td[4]";
        private const string TableSelectInsurancePayerName = "//*[@id=\"Insurances\"]/div[2]/table/tbody/tr/td[2]";
        private const string TableSelectInsuranceMapping = "//*[@id=\"MappingFilters\"]/div[2]/table/tbody/tr[4]";
        private const string ButtonUploadToDB = "Upload";
        private const string InsuranceToMapName = "BLUE CROSS/BLUE SHIELD - NASCO";
        private const string InsurancePayerId = "1000023014";
        private const string PayerTypeId = "4";

        // Test setup
        public override async Task InitAsync(object[] args)
        {
            await Driver.WaitUntilElementLoadsAsync(By.Id(HomePageLogoId));
        }

        /// <summary>
        /// Since the Insurance Mapping page has no link in the home page, we have to build the Url so that it is dynamic to account for the testing environment
        /// </summary>
        public async Task<InsuranceMappingPage> NavigateToInsuranceMappingPage()
        {
            Log.Step("Navigate to Insurance Mapping page.");
            string baseUrl = Driver.Url;
            Uri uri = new Uri(baseUrl);
            string hostUrl = uri.Host.ToString();
            string fullUrl = string.Format("https://{0}/InsuranceMapping/index", hostUrl);
            await Driver.NavigateAsync(fullUrl);
            return this;
        }

        public async Task<InsuranceMappingPage> ClickOnInsuranceToMap()
        {
            Log.Step("Select Insurance to Map.");
            await Driver.SelectDropDownOptionByValueAsync(DropdownState.XPath(), "FL");
            await Driver.SendKeysAsync(InputFilterInsuranceName.XPath(), "Blue Cross/" + Keys.Return);
            await Driver.ClickAsync(TableSelectInsuranceToMap.XPath());
            return this;
        }

        public async Task<InsuranceMappingPage> ClickOnInsurancePayerAsync()
        {
            Log.Step("Select Insurance Payer.");
            await Driver.SendKeysAsync(InputFilterInsuranceName2.XPath(), "Acordia" + Keys.Return);
            await Driver.ClickAsync(TableSelectInsurancePayerName.XPath());
            return this;
        }

        public async Task<InsuranceMappingPage> ClickOnInsurancePlanIdAsync()
        {
            Log.Step("Select Insurance Plan Id.");
            await Driver.ClickAsync(TableSelectInsuranceMapping.XPath());
            return this;
        }

        public async Task<bool> VerifyMappingBeforeUploadToDbAsync()
        {
            Log.Step("Verify all elements have been selected sucessfully.");
            var insuranceToMapName = await Driver.GetTextAsync(TableSelectInsuranceToMap.XPath());
            var insuranceToMapInsuranceId = await Driver.GetTextAsync(TableSelectInsuranceToMapInsuranceId.XPath());
            var insuranceToMapMappingFilterId = await Driver.GetTextAsync(TableSelectInsuranceToMapMappingFilterId.XPath());

            return insuranceToMapName == InsuranceToMapName && insuranceToMapInsuranceId == InsurancePayerId && insuranceToMapMappingFilterId == PayerTypeId;
        }

        public async Task<InsuranceMappingPage> ClickUploadButton()
        {
            Log.Step("Click Upload button");
            var verified = await VerifyMappingBeforeUploadToDbAsync();
            if (verified)
            {
                await Driver.ClickAsync(ButtonUploadToDB.Id());
                await Driver.WaitForAlertAndDismissAsync();
            }
            else
            {
                Log.Error("Mapping was unsuccessful");
            }
            return this;
        }
    }
}
