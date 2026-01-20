using OpenQA.Selenium;
using Serilog;
using VaxCare.Core;
using VaxCare.Core.Extensions;
using VaxCare.Core.Logger;
using VaxCare.Core.WebDriver;

namespace VaxCare.Pages.DataEntry
{
    /// <summary>
    /// DataEntry Home Page - Contains navigation methods and common page operations
    /// Note: This class currently consolidates functionality from multiple pages.
    /// Consider refactoring into separate page objects (DataEntryBatchProcessingPage, DataEntryConsentFormPage, etc.) as the framework grows.
    /// </summary>
    public class DataEntryHomePage(IWebDriver driver, ILogger log) : BasePage(driver, log)
    {
        private const string HomePageLogoId = "lnkConsentBatchIndex";
        
        // Navigation link IDs/XPaths - These will need to be identified from the actual application
        private const string BatchProcessingLink = "batchProcessingLink"; // Placeholder - update with actual selector
        private const string DataEntryLink = "dataEntryLink"; // Placeholder
        private const string DataEntryNoBatchLink = "dataEntryNoBatchLink"; // Placeholder
        private const string PatientSearchLink = "patientSearchLink"; // Placeholder
        private const string ConsentFormBatchEditLink = "consentFormBatchEditLink"; // Placeholder
        private const string InsuranceMappingLink = "insuranceMappingLink"; // Placeholder
        private const string LotNumberAdminToolLink = "lotNumberAdminToolLink"; // Placeholder
        private const string RcmPowerToolsLink = "rcmPowerToolsLink"; // Placeholder
        private const string FinancialReconciliationLink = "financialReconciliationLink"; // Placeholder
        private const string TransactionReportLink = "transactionReportLink"; // Placeholder
        private const string InventoryTransactionHistoryLink = "inventoryTransactionHistoryLink"; // Placeholder
        private const string PartnerHealthReportLink = "partnerHealthReportLink"; // Placeholder
        private const string VaccineCountManagerLink = "vaccineCountManagerLink"; // Placeholder
        private const string IntegrationsLink = "integrationsLink"; // Placeholder
        private const string PayspanFileGenerationLink = "payspanFileGenerationLink"; // Placeholder
        private const string DataUploadToolLink = "dataUploadToolLink"; // Placeholder
        private const string DevOpsToolsLink = "devOpsToolsLink"; // Placeholder
        private const string InReviewReportLink = "inReviewReportLink"; // Placeholder
        private const string MonthlyCompFinalizationLink = "monthlyCompFinalizationLink"; // Placeholder
        private const string HomeLink = "homeLink"; // Placeholder

        public override async Task InitAsync(object[] args)
        {
            await Driver.WaitUntilElementLoadsAsync(By.Id(HomePageLogoId), Log);
        }

        #region Navigation Methods

        public async Task<DataEntryHomePage> ClickHomeLink()
        {
            Log.Step("Click Home link.");
            await Driver.ClickAsync(HomeLink.Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> ClickBatchProcessingLink()
        {
            Log.Step("Click Batch Processing link.");
            await Driver.ClickAsync(BatchProcessingLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickDataEntryLink()
        {
            Log.Step("Click Data Entry link.");
            await Driver.ClickAsync(DataEntryLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickDataEntryNoBatchLink()
        {
            Log.Step("Click Data Entry No Batch link.");
            await Driver.ClickAsync(DataEntryNoBatchLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickPatientSearchLink()
        {
            Log.Step("Click Patient Search link.");
            await Driver.ClickAsync(PatientSearchLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickConsentFormBatchEditLink()
        {
            Log.Step("Click Consent Form Batch Edit link.");
            await Driver.ClickAsync(ConsentFormBatchEditLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickLotNumberAdminToolLink()
        {
            Log.Step("Click Lot Number Admin Tool link.");
            await Driver.ClickAsync(LotNumberAdminToolLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickRcmPowerToolsLink()
        {
            Log.Step("Click RCM Power Tools link.");
            await Driver.ClickAsync(RcmPowerToolsLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickFinancialReconciliationLink()
        {
            Log.Step("Click Financial Reconciliation link.");
            await Driver.ClickAsync(FinancialReconciliationLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickTransactionReportLink()
        {
            Log.Step("Click Transaction Report link.");
            await Driver.ClickAsync(TransactionReportLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickInventoryTransactionHistoryLink()
        {
            Log.Step("Click Inventory Transaction History link.");
            await Driver.ClickAsync(InventoryTransactionHistoryLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickPartnerHealthReportLink()
        {
            Log.Step("Click Partner Health Report link.");
            await Driver.ClickAsync(PartnerHealthReportLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickVaccineCountManagerLink()
        {
            Log.Step("Click Vaccine Count Manager link.");
            await Driver.ClickAsync(VaccineCountManagerLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickIntegrationsLink()
        {
            Log.Step("Click Integrations link.");
            await Driver.ClickAsync(IntegrationsLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickPayspanFileGenerationLink()
        {
            Log.Step("Click Payspan File Generation link.");
            await Driver.ClickAsync(PayspanFileGenerationLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickDataUploadToolLink()
        {
            Log.Step("Click Data Upload Tool link.");
            await Driver.ClickAsync(DataUploadToolLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickDevOpsToolsLink()
        {
            Log.Step("Click DevOps Tools link.");
            await Driver.ClickAsync(DevOpsToolsLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickInReviewReportLink()
        {
            Log.Step("Click In Review Report link.");
            await Driver.ClickAsync(InReviewReportLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickMonthlyCompFinalizationLink()
        {
            Log.Step("Click Monthly Comp Finalization link.");
            await Driver.ClickAsync(MonthlyCompFinalizationLink.Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> NavigateToInsuranceMappingPage()
        {
            Log.Step("Navigate to Insurance Mapping page.");
            string baseUrl = Driver.Url;
            Uri uri = new Uri(baseUrl);
            string hostUrl = uri.Host.ToString();
            string fullUrl = string.Format("https://{0}/InsuranceMapping/index", hostUrl);
            await Driver.NavigateAsync(fullUrl, Log);
            await Driver.WaitUntilElementLoadsAsync(By.Id("lnkConsentBatchIndex"), Log);
            return this;
        }

        public async Task<DataEntryHomePage> ClickOnInsuranceToMap()
        {
            Log.Step("Select Insurance to Map.");
            const string DropdownState = "//*[@id=\"MappingFallouts\"]/div[1]/table/tr[2]/td[2]/select";
            const string InputFilterInsuranceName = "//*[@id=\"MappingFallouts\"]/div[1]/table/tr[2]/td[1]/input";
            const string TableSelectInsuranceToMap = "//*[@id=\"MappingFallouts\"]/div[2]/table/tbody/tr[1]/td[1]";
            await Driver.SelectDropDownOptionByValueAsync(DropdownState.XPath(), "FL", Log);
            await Driver.SendKeysAsync(InputFilterInsuranceName.XPath(), "Blue Cross/" + Keys.Return, Log);
            await Driver.ClickAsync(TableSelectInsuranceToMap.XPath(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> ClickOnInsurancePayerAsync()
        {
            Log.Step("Select Insurance Payer.");
            const string InputFilterInsuranceName2 = "//*[@id=\"Insurances\"]/div[1]/table/tr[2]/td[2]/input";
            const string TableSelectInsurancePayerName = "//*[@id=\"Insurances\"]/div[2]/table/tbody/tr/td[2]";
            await Driver.SendKeysAsync(InputFilterInsuranceName2.XPath(), "Acordia" + Keys.Return, Log);
            await Driver.ClickAsync(TableSelectInsurancePayerName.XPath(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> ClickOnInsurancePlanIdAsync()
        {
            Log.Step("Select Insurance Plan Id.");
            const string TableSelectInsuranceMapping = "//*[@id=\"MappingFilters\"]/div[2]/table/tbody/tr[4]";
            await Driver.ClickAsync(TableSelectInsuranceMapping.XPath(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> ClickUploadButton()
        {
            Log.Step("Click Upload button");
            const string ButtonUploadToDB = "Upload";
            const string TableSelectInsuranceToMap = "//*[@id=\"MappingFallouts\"]/div[2]/table/tbody/tr[1]/td[1]";
            const string TableSelectInsuranceToMapInsuranceId = "//*[@id=\"MappingFallouts\"]/div[2]/table/tbody/tr[1]/td[3]";
            const string TableSelectInsuranceToMapMappingFilterId = "//*[@id=\"MappingFallouts\"]/div[2]/table/tbody/tr[1]/td[4]";
            const string InsuranceToMapName = "BLUE CROSS/BLUE SHIELD - NASCO";
            const string InsurancePayerId = "1000023014";
            const string PayerTypeId = "4";

            var insuranceToMapName = await Driver.GetTextAsync(TableSelectInsuranceToMap.XPath(), Log);
            var insuranceToMapInsuranceId = await Driver.GetTextAsync(TableSelectInsuranceToMapInsuranceId.XPath(), Log);
            var insuranceToMapMappingFilterId = await Driver.GetTextAsync(TableSelectInsuranceToMapMappingFilterId.XPath(), Log);

            var verified = insuranceToMapName == InsuranceToMapName && insuranceToMapInsuranceId == InsurancePayerId && insuranceToMapMappingFilterId == PayerTypeId;
            if (verified)
            {
                await Driver.ClickAsync(ButtonUploadToDB.Id(), Log);
                await Driver.WaitForAlertAndDismissAsync();
            }
            else
            {
                Log.Error("Mapping was unsuccessful");
            }
            return this;
        }

        #endregion

        #region Batch Processing Methods

        public async Task<DataEntryHomePage> ClickTheAddBatchButton()
        {
            Log.Step("Click the Add Batch button.");
            await Driver.ClickAsync("addBatchButton".Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> EnterValueIntoBatchRecId(int batchRecId)
        {
            Log.Step($"Enter batch Rec ID: {batchRecId}.");
            await Driver.SendKeysAsync("batchRecId".Id(), batchRecId.ToString(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> SaveBatchRegistration()
        {
            Log.Step("Save batch registration.");
            await Driver.ClickAsync("saveBatchButton".Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ExpandBatchReconciliationSectionAndVerifyItLoads()
        {
            Log.Step("Expand Batch Reconciliation section and verify it loads.");
            await Driver.ClickAsync("batchReconciliationSection".Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ExpandScanningAndVerificationSectionAndVerifyItLoads()
        {
            Log.Step("Expand Scanning and Verification section and verify it loads.");
            await Driver.ClickAsync("scanningVerificationSection".Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> ClickSaveAndFindAnotherButton()
        {
            Log.Step("Click Save and Find Another button.");
            await Driver.ClickAsync("saveAndFindAnotherButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> VerifyPageRedirectsToBatchProcessingList()
        {
            Log.Step("Verify page redirects to Batch Processing list.");
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> SearchForBatch(string searchTerm)
        {
            Log.Step($"Search for batch: {searchTerm}.");
            await Driver.SendKeysAsync("batchSearchInput".Id(), searchTerm, Log);
            await Driver.ClickAsync("batchSearchButton".Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        #endregion

        #region Consent Form Methods

        public async Task<DataEntryHomePage> CreateNewConsentForm()
        {
            Log.Step("Create new consent form.");
            await Driver.ClickAsync("newConsentFormButton".Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> FillOutConsentFormWithNecessaryInformation()
        {
            Log.Step("Fill out consent form with necessary information.");
            // Implementation would fill out form fields
            // This is a placeholder - actual implementation needed
            return this;
        }

        public async Task<DataEntryHomePage> VerifyPaymentAndInsuranceInformationSectionWasAutoPopulated()
        {
            Log.Step("Verify payment and insurance information section was auto-populated.");
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            // Add verification logic
            return this;
        }

        public async Task<DataEntryHomePage> AddVaccinationAndSave()
        {
            Log.Step("Add vaccination and save.");
            await Driver.ClickAsync("addVaccinationButton".Id(), Log);
            // Fill vaccination details
            await Driver.ClickAsync("saveConsentFormButton".Id(), Log);
            await Driver.WaitForAlertAndDismissAsync();
            return this;
        }

        public async Task<DataEntryHomePage> NavigateToDataEntryPage()
        {
            Log.Step("Navigate to Data Entry page.");
            await Driver.ClickAsync("dataEntryPageLink".Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> VerifyNewConsentFormIsListed()
        {
            Log.Step("Verify new consent form is listed.");
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            // Add verification logic
            return this;
        }

        public async Task<DataEntryHomePage> ClickViewLinkForFirstRow()
        {
            Log.Step("Click view link for first row.");
            await Driver.ClickAsync("firstRowViewLink".Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> VerifyConsentIdFieldIsPresent()
        {
            Log.Step("Verify consent ID field is present.");
            var isPresent = await Driver.IsElementPresentAsync("consentIdField".Id(), Log);
            if (!isPresent)
                throw new Exception("Consent ID field is not present.");
            return this;
        }

        public async Task<DataEntryHomePage> SelectPartnerAndClinicDataEntryNoBatch()
        {
            Log.Step("Select partner and clinic for Data Entry No Batch.");
            await Driver.SelectDropDownOptionByValueAsync("partnerDropdown".Id(), "1", Log); // Placeholder
            await Driver.SelectDropDownOptionByValueAsync("clinicDropdown".Id(), "1", Log); // Placeholder
            await Driver.ClickAsync("selectPartnerClinicButton".Id(), Log);
            return this;
        }

        #endregion

        #region Patient Search Methods

        public async Task<DataEntryHomePage> SearchFor(string searchTerm)
        {
            Log.Step($"Search for: {searchTerm}.");
            await Driver.SendKeysAsync("patientSearchInput".Id(), searchTerm, Log);
            await Driver.ClickAsync("patientSearchButton".Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> AddNote()
        {
            Log.Step("Add note.");
            await Driver.ClickAsync("addNoteButton".Id(), Log);
            await Driver.SendKeysAsync("noteTextArea".Id(), "Test note", Log);
            await Driver.ClickAsync("saveNoteButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> VerifyNoteWasAdded()
        {
            Log.Step("Verify note was added.");
            var isPresent = await Driver.IsTextPresentAsync("notesList".Id(), "Test note", Log);
            if (!isPresent)
                throw new Exception("Note was not added.");
            return this;
        }

        public async Task<DataEntryHomePage> CreateNewTransaction()
        {
            Log.Step("Create new transaction.");
            await Driver.ClickAsync("newTransactionButton".Id(), Log);
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        public async Task<DataEntryHomePage> EditOtherInsuranceNameAndSave()
        {
            Log.Step("Edit other insurance name and save.");
            await Driver.SendKeysAsync("otherInsuranceNameInput".Id(), "Updated Insurance Name", Log);
            await Driver.ClickAsync("saveInsuranceButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> ExitTheConsentFormPage()
        {
            Log.Step("Exit the consent form page.");
            await Driver.ClickAsync("exitButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> VerifyPageHeader()
        {
            Log.Step("Verify page header.");
            // TODO: Add WaitUntilElementLoadsAsync with actual selector once identified
            return this;
        }

        #endregion

        #region Consent Form Batch Edit Methods

        public async Task<DataEntryHomePage> EnterConsentFormsNumbers()
        {
            Log.Step("Enter consent forms numbers.");
            await Driver.SendKeysAsync("consentFormsNumbersInput".Id(), "12345,12346", Log); // Placeholder
            return this;
        }

        public async Task<DataEntryHomePage> EnterValuesForEligibilityIdAndNotes()
        {
            Log.Step("Enter values for eligibility ID and notes.");
            await Driver.SendKeysAsync("eligibilityIdInput".Id(), "123", Log); // Placeholder
            await Driver.SendKeysAsync("notesInput".Id(), "Test notes", Log); // Placeholder
            return this;
        }

        public async Task<DataEntryHomePage> SaveThePage()
        {
            Log.Step("Save the page.");
            await Driver.ClickAsync("saveBatchEditButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> VerifyConsentFormsNumbersAreInProperBoxes()
        {
            Log.Step("Verify consent forms numbers are in proper boxes.");
            // Add verification logic
            return this;
        }

        public async Task<DataEntryHomePage> VerifyConsentFormsModifiedValue()
        {
            Log.Step("Verify consent forms modified value.");
            // Add verification logic
            return this;
        }

        public async Task<DataEntryHomePage> VerifyNoteIsCorrect()
        {
            Log.Step("Verify note is correct.");
            var isPresent = await Driver.IsTextPresentAsync("notesDisplay".Id(), "Test notes", Log);
            if (!isPresent)
                throw new Exception("Note is not correct.");
            return this;
        }

        public async Task<DataEntryHomePage> ResetPageAndVerifyFieldsAreCleared()
        {
            Log.Step("Reset page and verify fields are cleared.");
            await Driver.ClickAsync("resetButton".Id(), Log);
            // Add verification logic
            return this;
        }

        public async Task<DataEntryHomePage> TestPartnerBillPaymentMode()
        {
            Log.Step("Test Partner Bill payment mode.");
            await Driver.SelectDropDownOptionByValueAsync("paymentModeDropdown".Id(), "PartnerBill", Log);
            await Driver.ClickAsync("applyPaymentModeButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> TestInsurancePayPaymentMode()
        {
            Log.Step("Test Insurance Pay payment mode.");
            await Driver.SelectDropDownOptionByValueAsync("paymentModeDropdown".Id(), "InsurancePay", Log);
            await Driver.ClickAsync("applyPaymentModeButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> TestSelfPayPaymentMode()
        {
            Log.Step("Test Self Pay payment mode.");
            await Driver.SelectDropDownOptionByValueAsync("paymentModeDropdown".Id(), "SelfPay", Log);
            await Driver.ClickAsync("applyPaymentModeButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> TestEmployerPayPaymentMode()
        {
            Log.Step("Test Employer Pay payment mode.");
            await Driver.SelectDropDownOptionByValueAsync("paymentModeDropdown".Id(), "EmployerPay", Log);
            await Driver.ClickAsync("applyPaymentModeButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> TestNoPayPaymentMode()
        {
            Log.Step("Test No Pay payment mode.");
            await Driver.SelectDropDownOptionByValueAsync("paymentModeDropdown".Id(), "NoPay", Log);
            await Driver.ClickAsync("applyPaymentModeButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> TestUpdateClaimStatus()
        {
            Log.Step("Test update claim status.");
            await Driver.SelectDropDownOptionByValueAsync("claimStatusDropdown".Id(), "Updated", Log);
            await Driver.ClickAsync("applyClaimStatusButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> TestUpdateInvalidStock()
        {
            Log.Step("Test update invalid stock.");
            await Driver.SelectDropDownOptionByValueAsync("stockStatusDropdown".Id(), "Invalid", Log);
            await Driver.ClickAsync("applyStockStatusButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> TestUpdateValidStock()
        {
            Log.Step("Test update valid stock.");
            await Driver.SelectDropDownOptionByValueAsync("stockStatusDropdown".Id(), "Valid", Log);
            await Driver.ClickAsync("applyStockStatusButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> TestHoldCallsAndStatementsUpdate()
        {
            Log.Step("Test hold calls and statements update.");
            await Driver.ClickAsync("holdCallsCheckbox".Id(), Log);
            await Driver.ClickAsync("holdStatementsCheckbox".Id(), Log);
            await Driver.ClickAsync("applyHoldButton".Id(), Log);
            return this;
        }

        public async Task<DataEntryHomePage> TestClearExistingDenialActionCompleted()
        {
            Log.Step("Test clear existing denial action completed.");
            await Driver.ClickAsync("clearDenialActionButton".Id(), Log);
            return this;
        }

        #endregion
    }
}

