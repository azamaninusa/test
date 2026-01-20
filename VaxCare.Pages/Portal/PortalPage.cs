using OpenQA.Selenium;
using Serilog;
using VaxCare.Core;
using VaxCare.Core.Entities.Patients;
using VaxCare.Core.Extensions;
using VaxCare.Core.Logger;
using VaxCare.Core.WebDriver;

namespace VaxCare.Pages.Portal
{
    public class PortalPage(IWebDriver driver, ILogger log) : BasePage(driver, log)
    {
        private IWebElement? TestPatientInSchedule { get; set; }
        private List<IWebElement> VisitsInSchedule { get; set; } = [];

        private const string LoginErrorMessage = "//*[@id=\"form53\"]/div[1]/div[1]/div/div/p";
        private const string NoPatientsMessage = "//div[contains(text(), 'No Patients in Schedule')]";
        private const string EmptyScheduleAddPatientsButton = "add-patients-to-schedule-no-patients";
        private const string AppointmentTable = "data-table";
        private const string AppointmentRow = "//tr[contains(@class,'appt-row')]";
        private const string AppointmentGrid = "//*[@class='ng-star-inserted']";
        private const string ArrowButton = "//button[contains(@class,'button-forward')]";
        private const string EditAppointmentButton = "editAppointmentBtn";
        private const string DeleteButton = "deleteButton";
        private const string SaveButton = "(//button[@id='saveButton'])[2]";
        private const string ClosePatientInfoWindowButton = "close";
        private const string AddAppointmentButton = "add-patients-to-schedule";
        private const string DoneButton = "done-action-button";
        private const string AddPatientButton = "add-patients-to-schedule-sub-header";
        private const string AddPatientWindow = "AppointmentCreateModal";
        private const string SearchPatientTextBox = "searchPatientInput";
        private const string ProviderTextBoxId = "provider";
        private const string PatientInSchedule = "//span[text()= '{0}']";
        private const string EligibilityInProgressText = "//div[contains(@class, 'loadingMessage')]";
        private const string RiskIcon = "//*[@id=\"eligibility-icon\"]/span[contains(@class,'{0}')]";
        private const string PatientInfoRiskDescriptionCN = "eligibility-description";
        private const string PatientInfoRiskDetails = "//p[contains(@class,'fade eligibility-details')]";
        private const string ScheduleRiskDescriptionCN = "eligibilitydescription";
        private const string ScheduleRiskDetails = ".//div[contains(@class,'eligibilitydetails')]";
        private const string PatientInSearchBox = "//div[contains(@id,'cdk-overlay')]//span[@class='patient-name']/span[text()='{0}']";
        private const string ProviderNameInSearch = "//span[contains(text(), '{0}')]";
        private const string DivWithText = "//div[contains(text(), '{0}')]";
        private const string LoadingIcon = "app-loading-image";
        private const string DatePicker = "datepickerTitle";
        private const string RunMedDButton = "run-med-d";
        private const string EditPaymentInfoButton = "save";
        private const string MedDCopayCheckWindow = "vaccine-copay-table";
        private const string MedDNameOnCardTextField = "ccName";
        private const string MedDNameOnCardValue = "QA Robot";
        private const string MedDCardTextField = "ccNumber";
        private const string MedDCardNumberValue = "4111111111111111";
        private const string MedDExpirationDateTextField = "ccExpirationDate";
        private const string MedDEmailTextField = "ccEmail";
        private const string MedDEmailAddressValue = "qauser@vaxcare.com";
        private const string MedDPhoneNumberTextField = "ccPhone";
        private const string MedDPhoneNumberValue = "407-555-1234";
        private const string MedDSavePaymentInfoButton = "//button[@id='saveButton']//div[text()='Save Payment Info']";
        private const string MedStatusCheckComplete = "//div[contains(text(), 'Medicare Part D check complete.')]";
        private const string MedDRiskDetails = "//*/div/app-appointments/div/div/div[2]/div[1]/app-appointment-messaging/div/div/div[2]/app-appointment-encounter-message/div/div/div/p";
        private const string MedDCopay = "//*[@id='call-to-action-meddcollectsignature']/app-med-d-result/div/div/div[2]/p/b/ul/span/li";

        public async Task<bool> ConfirmGracefulLoginFailedMessageAsync()
        {
            Log.Step("Confirm that login fails gracefully");
            var result = await Driver.IsTextPresentAsync(LoginErrorMessage.XPath(), "Unable to sign in");
            return result;
        }

        public async Task<bool> IsScheduleEmptyAsync()
        {
            Log.Step("Step: Check if schedule is empty.");

            try
            {
                var emptyScheduleMessage = await Driver.IsElementPresentAsync(NoPatientsMessage.XPath(), Log);
                var addPatientsButtonExists = await Driver.IsElementClickableAsync(EmptyScheduleAddPatientsButton.Id(), Log);
                return emptyScheduleMessage && addPatientsButtonExists;
            }
            catch (WebDriverTimeoutException)
            {
                Log.Warning("Schedule may not be empty. Checking for patients next");
                var patientsInSchedule = await PatientVisitsExistInScheduleAsync();

                if (patientsInSchedule)
                    Log.Error("Test Setup Error: The schedule has patients.");
                else
                {
                    Log.Error("Schedule failed to load.");
                    await Driver.CheckBrowserConsoleErrorsAsync(Log);
                }
                return false;
            }
        }

        public async Task<bool> PatientVisitsExistInScheduleAsync()
        {
            Log.Step("Check if patients exist in the schedule.");
            VisitsInSchedule = await Driver.FindAllElementsAsync(AppointmentTable.Id(), AppointmentRow.XPath(), Log);
            return VisitsInSchedule.Count > 0;
        }

        public async Task<PortalPage> WaitForAppointmentGridToLoadAsync()
        {
            int timeout = 40;
            Log.Step("Step: Wait for appointment grid to load.");
            await Driver.WaitForElementToDisappearAsync(LoadingIcon.ClassName(), Log, timeout);
            await Driver.WaitUntilElementLoadsAsync(DatePicker.ClassName(), Log, timeout);
            //await Driver.CheckBrowserConsoleErrorsAsync(Log);
            return this;
        }

        public async Task<PortalPage> FindPatientInScheduleAsync(string lastName)
        {
            Log.Step($"Find Patient with Last Name: '{lastName}' in the schedule");
            TestPatientInSchedule = await Driver.FindElementAsync(string.Format(DivWithText, lastName).XPath(), Log, 15);
            return this;
        }

        public async Task<bool> IsPatientEligibilityCorrectAsync(TestPatient patient)
        {
            Log.Step("Check if patient eligibility is correct.");
            var correctSchedulerRiskStatus = false;
            var correctPatientInfoRiskStatus = false;
            await FindPatientInScheduleAsync(patient.LastName);
            await WaitForEligibilityToRunAsync();

            if (TestPatientInSchedule != null)
            {
                correctSchedulerRiskStatus = await CheckRiskStatus(patient, patient.EligibilityStatus, false);
                await Driver.ClickAsync(TestPatientInSchedule, Log);
                correctPatientInfoRiskStatus |= await CheckRiskStatus(patient, patient.EligibilityStatus, true);
            }
            return correctSchedulerRiskStatus || correctPatientInfoRiskStatus;
        }

        public async Task<PortalPage> WaitForEligibilityToRunAsync()
        {
            Log.Step("Waiting for Eligibility to Run.");
            await Driver.WaitForElementToDisappearAsync(EligibilityInProgressText.XPath(), Log);
            return this;
        }

        public async Task<bool> CheckRiskStatus(TestPatient patient, EligibilityStatus eligStatus, bool inPatientInformationWindow = false)
        {
            var isIconCorrect = await CheckRiskIconAsync(eligStatus);
            var isTextCorrect = await CheckRiskTextAsync(patient, inPatientInformationWindow);

            return isIconCorrect || isTextCorrect;
        }

        public async Task<bool> CheckRiskIconAsync(EligibilityStatus eligStatus)
        {
            var riskIconClassName = await GetRiskIconClassName(eligStatus);
            await Driver.WaitUntilElementLoadsAsync(string.Format(RiskIcon, riskIconClassName).XPath(), Log);

            return await Driver.IsElementPresentAsync(string.Format(RiskIcon, riskIconClassName).XPath(), Log);
        }

        private Task<string> GetRiskIconClassName(EligibilityStatus eligStatus)
        {
            string riskIconClassName;
            switch (eligStatus)
            {
                case EligibilityStatus.RiskFullMoon:
                    riskIconClassName = " ";
                    break;
                case EligibilityStatus.RiskFreePreShot:
                    riskIconClassName = "vax-icon-ic_eligibility_risk_free";
                    break;
                case EligibilityStatus.RiskHalfMoon:
                    riskIconClassName = "half_moon_pre_shot.png";
                    break;
                case EligibilityStatus.AtRiskDataComplete:
                    riskIconClassName = "bluecircle.png";
                    break;
                case EligibilityStatus.AtRiskDataMissing:
                    riskIconClassName = "at_risk_pre_shot.png";
                    break;
                case EligibilityStatus.AtRiskDataIncorrect:
                    riskIconClassName = "not_eligible_pre_shot.png";
                    break;
                case EligibilityStatus.MissingOrInvalidPayerName:
                    riskIconClassName = "vax-icon-ic_eligibility_half";
                    break;
                case EligibilityStatus.PartnerBill:
                    riskIconClassName = "vax-icon-ic_eligibility_full";
                    break;

                default:
                    Log.Error("CODE ERROR: Didn't implement the Eligibility Status that is being searched for.");
                    throw new Exception();
            }
            return Task.FromResult(riskIconClassName);
        }

        public async Task<bool> CheckRiskTextAsync(TestPatient patient, bool inPatientInformationWindow = false, bool isMedDCheck = false)
        {

            string riskDescriptionSelector = ScheduleRiskDescriptionCN;
            string riskDetailselector = ScheduleRiskDetails;
            string? riskDetails;
            string? riskDescription;
            var descriptionMatches = false;
            var detailsMatch = false;

            if (inPatientInformationWindow)
            {
                riskDescriptionSelector = PatientInfoRiskDescriptionCN;
                riskDetailselector = PatientInfoRiskDetails;
            }

            if (isMedDCheck)
            {
                riskDetailselector = MedDRiskDetails;
            }

            riskDescription = await Driver.GetTextAsync(riskDescriptionSelector.ClassName(), Log);
            descriptionMatches = riskDescription.Equals(patient.RiskDescription);
            riskDetails = await Driver.GetTextAsync(riskDetailselector.XPath(), Log);
            detailsMatch = riskDetails.Equals(patient.FadeRiskDetails);

            //Disregard if details match in this case
            if (patient.RiskDetails.Equals("N/A"))
            {
                detailsMatch = true;
            }

            if (!descriptionMatches)
                Log.Error("Risk Description does not match the expected value");
            else if (!detailsMatch)
                Log.Error("Risk details do not match the expected value");

            return descriptionMatches && detailsMatch;
        }

        public async Task<PortalPage> CheckMedDStatusAsync()
        {
            Log.Step("Open Patient Information Window and check Med D status.");
            await Driver.ClickAsync(TestPatientInSchedule!);

            var runMedDButtonExists = await Driver.IsElementPresentAsync(RunMedDButton.Id(), Log, 25);

            if (runMedDButtonExists)
            {
                await RunMedDEligibilityAsync();
            }

            return this;
        }

        public async Task<PortalPage> RunMedDEligibilityAsync()
        {
            Log.Step("Run MedD Eligibility");
            await Driver.ClickAsync(RunMedDButton.Id(), Log);
            await Driver.WaitUntilElementLoadsAsync(MedDCopayCheckWindow.Id(), 20);
            await FillOutCopayFormAsync();
            await Driver.ClickAsync(MedDSavePaymentInfoButton.XPath(), Log);
            return this;
        }

        public async Task FillOutCopayFormAsync()
        {
            Log.Step("Fill out Copay Check Form");
            var expirationDate = DateTime.Now.AddYears(1).ToString("MM/yy");
            await Driver.SendKeysAsync(MedDCardTextField.Id(), MedDCardNumberValue, Log);
            await Driver.SendKeysAsync(MedDNameOnCardTextField.Id(), MedDNameOnCardValue, Log);
            await Driver.SendKeysAsync(MedDExpirationDateTextField.Id(), expirationDate, Log);
            await Driver.SendKeysAsync(MedDPhoneNumberTextField.Id(), MedDPhoneNumberValue, Log);
            await Driver.SendKeysAsync(MedDEmailTextField.Id(), MedDEmailAddressValue, Log);
        }

        public async Task<bool> IsMedDCorrectAsync(TestPatient patient)
        {
            Log.Step("Check Med D status matches expectations.");
            await Driver.WaitForElementToDisappearAsync(string.Format(DivWithText, "Initial Eligibility Check in Progress").XPath(), Log, 30);
            var correctRisk = await CheckRiskTextAsync(patient, true, true);

            var correctMedDEligDescription = await Driver.IsElementPresentAsync(string.Format(DivWithText, "Med D").XPath(), Log);
            var correctMedDStatusCheck = await Driver.IsElementPresentAsync(MedStatusCheckComplete.XPath(), Log);
            await Driver.WaitForElementToDisappearAsync(string.Format(DivWithText, "Initial Eligibility Check in Progress").XPath(), Log, 30);
            var medDCopay = await Driver.GetTextAsync(MedDCopay.XPath(), Log, 15);

            var correctMedDCopay = medDCopay.Equals(patient.MedDCopay);

            if (!correctMedDCopay)
                Log.Error("Med D copay does not match the expected value");

            return correctRisk && correctMedDEligDescription && correctMedDStatusCheck && correctMedDCopay;
        }
    }
}
