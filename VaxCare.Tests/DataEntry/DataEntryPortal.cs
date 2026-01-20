using VaxCare.Core;
using VaxCare.Core.Extensions;
using VaxCare.Pages.DataEntry;
using VaxCare.Pages.Login;
using Xunit;
using Xunit.Abstractions;

namespace VaxCare.Tests.DataEntry
{
    public class DataEntryPortal : BaseTest
    {
        public DataEntryPortal(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("https://dataentrystg.vaxcare.com/")]
        public async Task CreateBatch(string logInUrl)
        {
            await RunTestAsync("Create Batch", async () =>
            {
                await PageAsync<DataEntryLogin>()
                    .Then(page => page.LoginAsync(logInUrl))
                    .Then(page => page.ClickBatchProcessingLink())
                    .Then(page => page.ClickTheAddBatchButton())
                    .Then(page => page.EnterValueIntoBatchRecId(139139))
                    .Then(page => page.SaveBatchRegistration())
                    .Then(page => page.ExpandBatchReconciliationSectionAndVerifyItLoads())
                    .Then(page => page.ExpandScanningAndVerificationSectionAndVerifyItLoads())
                    .Then(page => page.ClickSaveAndFindAnotherButton())
                    .Then(page => page.VerifyPageRedirectsToBatchProcessingList());
            });
        }

        [Theory]
        [InlineData("https://dataentrystg.vaxcare.com/")]
        public async Task BatchSearchFunctionality(string logInUrl)
        {
            await RunTestAsync("Batch Search Functionality", async () =>
            {
                await PageAsync<DataEntryLogin>()
                    .Then(page => page.LoginAsync(logInUrl))
                    .Then(page => page.ClickBatchProcessingLink())
                    .Then(page => page.SearchForBatch("Middleton"));
            });
        }

        [Theory]
        [InlineData("https://dataentrystg.vaxcare.com/")]
        public async Task ConsentFormFunctionality(string logInUrl)
        {
            await RunTestAsync("Consent Form Functionality", async () =>
            {
                await PageAsync<DataEntryLogin>()
                    .Then(page => page.LoginAsync(logInUrl))
                    .Then(page => page.ClickDataEntryLink())
                    .Then(page => page.CreateNewConsentForm())
                    .Then(page => page.FillOutConsentFormWithNecessaryInformation())
                    .Then(page => page.VerifyPaymentAndInsuranceInformationSectionWasAutoPopulated())
                    .Then(page => page.AddVaccinationAndSave())
                    .Then(page => page.NavigateToDataEntryPage())
                    .Then(page => page.VerifyNewConsentFormIsListed())
                    .Then(page => page.ClickViewLinkForFirstRow())
                    .Then(page => page.VerifyConsentIdFieldIsPresent());
            });
        }

        [Theory]
        [InlineData("https://dataentrystg.vaxcare.com/")]
        public async Task ConsentFormFunctionalityDataEntryNoBatch(string logInUrl)
        {
            await RunTestAsync("Consent Form Functionality DataEntry No Batch", async () =>
            {
                await PageAsync<DataEntryLogin>()
                    .Then(page => page.LoginAsync(logInUrl))
                    .Then(page => page.ClickDataEntryNoBatchLink())
                    .Then(page => page.SelectPartnerAndClinicDataEntryNoBatch())
                    .Then(page => page.CreateNewConsentForm())
                    .Then(page => page.FillOutConsentFormWithNecessaryInformation())
                    .Then(page => page.VerifyPaymentAndInsuranceInformationSectionWasAutoPopulated())
                    .Then(page => page.AddVaccinationAndSave())
                    .Then(page => page.NavigateToDataEntryPage())
                    .Then(page => page.VerifyNewConsentFormIsListed())
                    .Then(page => page.ClickViewLinkForFirstRow())
                    .Then(page => page.VerifyConsentIdFieldIsPresent());
            });
        }

        [Theory]
        [InlineData("https://dataentrystg.vaxcare.com/")]
        public async Task PatientSearch(string logInUrl)
        {
            await RunTestAsync("Patient Search", async () =>
            {
                await PageAsync<DataEntryLogin>()
                    .Then(page => page.LoginAsync(logInUrl))
                    .Then(page => page.ClickPatientSearchLink())
                    .Then(page => page.SearchFor("1794659"))
                    .Then(page => page.ClickViewLinkForFirstRow())
                    .Then(page => page.AddNote())
                    .Then(page => page.VerifyNoteWasAdded())
                    .Then(page => page.CreateNewTransaction())
                    .Then(page => page.EditOtherInsuranceNameAndSave())
                    .Then(page => page.AddVaccinationAndSave())
                    .Then(page => page.ExitTheConsentFormPage())
                    .Then(page => page.VerifyPageHeader());
            });
        }

        [Theory]
        [InlineData("https://dataentrystg.vaxcare.com/")]
        public async Task ConsentFormBatchEditSmokeTest(string logInUrl)
        {
            await RunTestAsync("Consent Form Batch Edit Smoke Test", async () =>
            {
                await PageAsync<DataEntryLogin>()
                    .Then(page => page.LoginAsync(logInUrl))
                    .Then(page => page.ClickConsentFormBatchEditLink())
                    .Then(page => page.EnterConsentFormsNumbers())
                    .Then(page => page.EnterValuesForEligibilityIdAndNotes())
                    .Then(page => page.SaveThePage())
                    .Then(page => page.VerifyConsentFormsNumbersAreInProperBoxes())
                    .Then(page => page.VerifyConsentFormsModifiedValue())
                    .Then(page => page.VerifyNoteIsCorrect())
                    .Then(page => page.ResetPageAndVerifyFieldsAreCleared())
                    .Then(page => page.TestPartnerBillPaymentMode())
                    .Then(page => page.TestInsurancePayPaymentMode())
                    .Then(page => page.TestSelfPayPaymentMode())
                    .Then(page => page.TestEmployerPayPaymentMode())
                    .Then(page => page.TestNoPayPaymentMode())
                    .Then(page => page.TestUpdateClaimStatus())
                    .Then(page => page.TestUpdateInvalidStock())
                    .Then(page => page.TestUpdateValidStock())
                    .Then(page => page.TestHoldCallsAndStatementsUpdate())
                    .Then(page => page.TestClearExistingDenialActionCompleted());
            });
        }

        [Theory]
        [InlineData("https://dataentrystg.vaxcare.com/")]
        public async Task InsuranceMappingRegression(string logInUrl)
        {
            await RunTestAsync("Insurance Mapping Regression", async () =>
            {
                await PageAsync<DataEntryLogin>()
                    .Then(page => page.LoginAsync(logInUrl))
                    .Then(page => page.NavigateToInsuranceMappingPage())
                    .Then(page => page.ClickOnInsuranceToMap())
                    .Then(page => page.ClickOnInsurancePayerAsync())
                    .Then(page => page.ClickOnInsurancePlanIdAsync())
                    .Then(page => page.ClickUploadButton());
            });
        }

        [Theory]
        [InlineData("https://dataentrystg.vaxcare.com/")]
        public async Task BasicSmokeTest(string logInUrl)
        {
            await RunTestAsync("Basic Smoke Test", async () =>
            {
                await PageAsync<DataEntryLogin>()
                    .Then(page => page.LoginAsync(logInUrl))
                    .Then(page => page.ClickBatchProcessingLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickDataEntryLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickDataEntryNoBatchLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickPatientSearchLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickInventoryTransactionHistoryLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickPartnerHealthReportLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickVaccineCountManagerLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickLotNumberAdminToolLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickConsentFormBatchEditLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickMonthlyCompFinalizationLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickIntegrationsLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickPayspanFileGenerationLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickDataUploadToolLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickRcmPowerToolsLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickDevOpsToolsLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickFinancialReconciliationLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickTransactionReportLink())
                    .Then(page => page.ClickHomeLink())
                    .Then(page => page.ClickInReviewReportLink());
            });
        }
    }
}





