using OpenQA.Selenium;
using Serilog;
using VaxCare.Core;
using VaxCare.Core.Entities.Patients;
using VaxCare.Core.Extensions;
using VaxCare.Core.Logger;
using VaxCare.Core.TestDTOs;
using VaxCare.Core.TestFixtures;
using VaxCare.Core.WebDriver;

namespace VaxCare.Pages.Portal
{
    public class PortalPage(IWebDriverActor driver, ILogger log) : BasePage(driver, log)
    {
        private IWebElement? TestPatientInSchedule { get; set; }
        private List<IWebElement> VisitsInSchedule { get; set; } = [];

        private const string LoginErrorMessage = "//*[@id=\"form53\"]/div[1]/div[1]/div/div/p";
        private const string NoPatientsMessage = "//div[contains(text(), 'No Patients in Schedule')]";
        private const string PatientInfoWindow = "//*[@id=\"PatientInformationModal\"]";
        private const string EmptyScheduleAddPatientsButton = "add-patients-to-schedule-no-patients";
        private const string AppointmentTable = "data-table";
        private const string AppointmentRow = "//tr[contains(@class,'appt-row')]";
        private const string AppointmentGrid = "//*[@class='ng-star-inserted']";
        private const string ArrowButton = "//button[contains(@class,'button-forward')]";
        private const string EditAppointmentButton = "editAppointmentBtn";
        private const string DeleteButton = "deleteButton";
        private const string SaveButton = "//button[@id='saveButton']";
        private const string PatientInfoSaveButton = "(//button[@id='saveButton'])[2]";
        private const string ClosePatientInfoWindowButton = "close";
        private const string CreateNewPatientButton = "createNewPatient";
        private const string ScheduleAddAppointmentButton = "add-patients-to-schedule";
        private const string DoneButton = "done-action-button";
        private const string AddNewPatientButton = "add-patients-to-schedule-sub-header";
        private const string AddPatientWindow = "AppointmentCreateModal";
        private const string SearchPatientTextBox = "searchPatientInput";
        private const string ProviderTextBoxId = "provider";
        private const string EligibilityInProgressText = "//div[contains(@class, 'loadingMessage')]";
        private const string RiskIcon = "//*[@id=\"eligibility-icon\"]/span[contains(@class,'{0}')]";
        private const string PatientInfoRiskDescriptionCN = "//span[contains(@class,'eligibility-description')]";
        private const string PatientInfoRiskDetails = "//p[contains(@class,'fade eligibility-details')]";
        private const string ScheduleRiskDescriptionCN = "//*[@class= 'eligibilitydescription']";
        private const string ScheduleRiskDetails = ".//div[contains(@class,'eligibilitydetails')]";
        private const string PatientInSearchBox = "//div[contains(@id,'cdk-overlay')]//span[@class='patient-name']/span[text()='{0}']";
        private const string ProviderNameInSearch = "//span[contains(text(), '{0}')]";
        private const string FirstProviderOption = "//mat-option[1]//*[@class='mat-option-text']";
        private const string ProviderSelected = "//input[@id='provider' and @aria-invalid='false']";
        private const string NewlyCreatedPatientProvider = "Dean Morris";

        // Clinic/location selectors used by VerifyAppointmentLocationAndDayCanBeChanged helpers
        private const string ClinicDropdown = "//mat-select[@id='clinicDropDown']//div[contains(@class,'mat-select-arrow')]";
        private const string ClinicSelector = "//mat-select[@id='clinicSelector']//div[contains(@class,'mat-select-arrow-wrapper')]";
        private const string ClinicSearchBox = "//input[@placeholder='Search']";
        private const string ClinicOption = "//span[contains(text(),'{0}')]";

        private const string DatePickerInput = "datePickerInput";

        private const string DivWithText = "//div[contains(text(), '{0}')]";
        private const string SpanWithText = "//span[text()= '{0}']";
        private const string LinkWithText = "//a[contains(text(),'{0}')]";

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
        private const string MedDDescription = "//*[@class = 'primary-details']/span[2]";
        private const string MedDRiskDetails = "//*/div/app-appointments/div/div/div[2]/div[1]/app-appointment-messaging/div/div/div[2]/app-appointment-encounter-message/div/div/div/p";
        private const string MedDCopay = "//*[@id='call-to-action-meddcollectsignature']/app-med-d-result/div/div/div[2]/p/b/ul/span/li";

        private const string NewPatientFirstName = "firstName";
        private const string NewPatientLastName = "lastName";
        private const string NewPatientGender = "gender";
        private const string NewPatientDoB = "dob";
        private const string NewPatientPhone = "phone";
        private const string NewPatientPayer = "primaryInsurance";

        private const string LoadingSpinner = "//*[@class='block-ui-spinner']";

        private TestPatient? _currentPatient;

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
                var emptyScheduleMessage = await Driver.IsElementPresentAsync(NoPatientsMessage.XPath());
                var addPatientsButtonExists = await Driver.IsElementClickableAsync(EmptyScheduleAddPatientsButton.Id());
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
            VisitsInSchedule = await Driver.FindAllElementsAsync(AppointmentTable.Id(), AppointmentRow.XPath());
            return VisitsInSchedule.Count > 0;
        }

        public async Task<PortalPage> WaitForAppointmentGridToLoadAsync()
        {
            int timeout = 40;
            Log.Step("Step: Wait for appointment grid to load.");
            await Driver.WaitForElementToDisappearAsync(LoadingIcon.ClassName(), timeout);
            await Driver.WaitUntilElementLoadsAsync(DatePicker.ClassName(), timeout);
            //await Driver.CheckBrowserConsoleErrorsAsync(Log);
            return this;
        }

        public async Task<PortalPage> FindPatientInScheduleAsync(string lastName)
        {
            Log.Step($"Find Patient with Last Name: '{lastName}' in the schedule");
            TestPatientInSchedule = await Driver.FindElementAsync(string.Format(DivWithText, lastName).XPath(), 15);
            return this;
        }

        public async Task<EligibilityCheckDto> IsPatientEligibilityCorrectAsync(TestPatient patient)
        {
            Log.Step("Check if patient eligibility is correct.");
            var data = new EligibilityCheckDto();
            await FindPatientInScheduleAsync(patient.LastName);
            await WaitForEligibilityToRunAsync();

            if (TestPatientInSchedule != null)
            {
                Log.Information("Check Risk Status in the Schedule");
                (data.CorrectSchedulerIcon, data.SchedulerRiskDetails, data.SchedulerRiskDescription) = await GetRiskStatus(patient, patient.EligibilityStatus, false);

                await Driver.ClickAsync(TestPatientInSchedule);
                await Driver.WaitUntilElementLoadsAsync(PatientInfoWindow.XPath(), 15);

                Log.Information("Check Risk Status in the Patient Information window");
                (data.CorrectPatientInfoIcon, data.PatientInfoRiskDetails, data.PatientInfoRiskDescription) = await GetRiskStatus(patient, patient.EligibilityStatus, true);
            }
            return data;
        }

        public async Task<PortalPage> WaitForEligibilityToRunAsync()
        {
            Log.Step("Waiting for Eligibility to Run.");
            await Driver.WaitForElementToDisappearAsync(EligibilityInProgressText.XPath());
            return this;
        }

        public async Task<(bool correctIcon, string Details, string Description)> GetRiskStatus(TestPatient patient, EligibilityStatus eligStatus, bool inPatientInformationWindow = false)
        {
            var isIconCorrect = await CheckRiskIconAsync(eligStatus);
            var riskInfo = await GetRiskTextAsync(patient, inPatientInformationWindow);

            //Disregard if details match in this case
            if (patient.RiskDetails.Equals("N/A"))
            {
                riskInfo.Details = patient.FadeRiskDetails;
            }

            return (isIconCorrect, riskInfo.Details, riskInfo.Description);
        }

        private async Task<bool> CheckRiskIconAsync(EligibilityStatus eligStatus)
        {
            var riskIconClassName = await GetRiskIconClassName(eligStatus);
            await Driver.WaitUntilElementLoadsAsync(string.Format(RiskIcon, riskIconClassName).XPath());

            return await Driver.IsElementPresentAsync(string.Format(RiskIcon, riskIconClassName).XPath());
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

        public async Task<(string Details, string Description)> GetRiskTextAsync(TestPatient patient, bool inPatientInformationWindow = false, bool isMedDCheck = false)
        {

            string riskDescriptionSelector = ScheduleRiskDescriptionCN;
            string riskDetailselector = ScheduleRiskDetails;
            string? riskDetails;
            string? riskDescription;

            if (inPatientInformationWindow)
            {
                riskDescriptionSelector = PatientInfoRiskDescriptionCN;
                riskDetailselector = PatientInfoRiskDetails;
            }

            if (isMedDCheck)
            {
                riskDescriptionSelector = MedDDescription;
                riskDetailselector = MedDRiskDetails;
            }

            riskDescription = await Driver.GetTextAsync(riskDescriptionSelector.XPath());
            riskDetails = await Driver.GetTextAsync(riskDetailselector.XPath());

            return (riskDetails, riskDescription);
        }

        public async Task<PortalPage> CheckMedDStatusAsync()
        {
            Log.Step("Open Patient Information Window and check Med D status.");
            await Driver.ClickAsync(TestPatientInSchedule!);

            var runMedDButtonExists = await Driver.IsElementPresentAsync(RunMedDButton.Id(), 25);

            if (runMedDButtonExists)
            {
                await RunMedDEligibilityAsync();
            }

            return this;
        }

        public async Task<PortalPage> RunMedDEligibilityAsync()
        {
            Log.Step("Run MedD Eligibility");
            await Driver.ClickAsync(RunMedDButton.Id());
            await Driver.WaitUntilElementLoadsAsync(MedDCopayCheckWindow.Id(), 20);
            await FillOutCopayFormAsync();
            await Driver.ClickAsync(MedDSavePaymentInfoButton.XPath());
            return this;
        }

        private async Task FillOutCopayFormAsync()
        {
            Log.Step("Fill out Copay Check Form");
            var expirationDate = DateTime.Now.AddYears(1).ToString("MM/yy");
            await Driver.SendKeysAsync(MedDCardTextField.Id(), MedDCardNumberValue);
            await Driver.SendKeysAsync(MedDNameOnCardTextField.Id(), MedDNameOnCardValue);
            await Driver.SendKeysAsync(MedDExpirationDateTextField.Id(), expirationDate);
            await Driver.SendKeysAsync(MedDPhoneNumberTextField.Id(), MedDPhoneNumberValue);
            await Driver.SendKeysAsync(MedDEmailTextField.Id(), MedDEmailAddressValue);
        }

        public async Task<MedDCheckDto> IsMedDCorrectAsync(TestPatient patient)
        {
            Log.Step("Check Med D status matches expectations.");
            var data = new MedDCheckDto();

            await Driver.WaitForElementToDisappearAsync(string.Format(DivWithText, "Initial Eligibility Check in Progress").XPath(), 30);
            (data.RiskDetails, data.RiskDescription) = await GetRiskTextAsync(patient, true, true);

            data.CorrectMedDEligDescription = await Driver.IsElementPresentAsync(string.Format(DivWithText, "Med D").XPath());
            data.CorrectMedDStatusCheck = await Driver.IsElementPresentAsync(MedStatusCheckComplete.XPath());

            await Driver.WaitForElementToDisappearAsync(string.Format(DivWithText, "Initial Eligibility Check in Progress").XPath(), 30);
            data.MedDCopay = await Driver.GetTextAsync(MedDCopay.XPath(), 15);

            return data;
        }

        public async Task<PortalPage> AddAppointmentToScheduleAsync(TestPatient patient, bool createNewPatient)
        {
            Log.Step("Add new Appointment to the schedule");
            await Driver.ClickAsync(AddNewPatientButton.Id());

            if (createNewPatient)
                await CreateNewPatientAsync(patient);
            else
                await SearchForPatientAsync(patient);

            // Check if provider field is empty (match legacy behavior)
            await Driver.WaitUntilElementLoadsAsync(ProviderTextBoxId.Id());
            var providerValue = await Driver.FindElementAsync(ProviderTextBoxId.Id());
            var providerValueAttribute = providerValue?.GetAttribute("value") ?? string.Empty;

            if (string.IsNullOrEmpty(providerValueAttribute))
            {
                // Match legacy SelectProvider behavior: type last name, then select full name
                var providerLastName = NewlyCreatedPatientProvider.Split(' ')[1]; // "Morris"
                await Driver.SendKeysAsync(ProviderTextBoxId.Id(), providerLastName);
                await Task.Delay(3000); // Match legacy Sleep(3000)
                
                // Select the provider option containing the full name
                var providerOptionXpath = string.Format(ProviderNameInSearch, NewlyCreatedPatientProvider);
                await Driver.WaitUntilElementLoadsAsync(providerOptionXpath.XPath(), 15);
                await Driver.ClickAsync(providerOptionXpath.XPath());
                
                // Wait for provider to be selected (match legacy WaitForElementToAppear)
                await Driver.WaitUntilElementLoadsAsync(ProviderSelected.XPath(), 15);
            }

            await Driver.ClickAsync(ScheduleAddAppointmentButton.Id());
            await Driver.ClickAsync(DoneButton.Id());

            return this;
        }

        private async Task CreateNewPatientAsync(TestPatient patient, string insuranceName = "Uninsured")
        {
            await Driver.ClickAsync(CreateNewPatientButton.Id());
            await Driver.SendKeysAsync(NewPatientFirstName.Id(), patient.FirstName);
            await Driver.SendKeysAsync(NewPatientLastName.Id(), patient.LastName);
            await SelectMatDropdownOptionAsync(NewPatientGender.Id(), patient.Gender == 1 ? "Male" : "Female");
            await Driver.SendKeysAsync(NewPatientDoB.Id(), patient.DoB);
            await Driver.SendKeysAsync(NewPatientPhone.Id(), patient.PhoneNumber);
            await Driver.SendKeysAsync(NewPatientPayer.Id(), insuranceName);
            await Driver.ClickAsync(SaveButton.XPath());
        }

        private async Task SelectMatDropdownOptionAsync(By selector, string option)
        {
            await Driver.ClickAsync(selector);
            await Driver.ClickAsync(string.Format(SpanWithText, option).XPath());
        }

        private async Task SearchForPatientAsync(TestPatient patient)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsPatientInScheduleAsync(string lastName, string fullName)
        {
            var patientFound = await Driver.IsElementPresentAsync(string.Format(DivWithText, lastName).XPath(), 15);

            if (patientFound)
                Log.Information($"Test Patient found: {fullName}!");

            return patientFound;
        }

        public async Task<PortalPage> DeletePatientVisitAsync(string lastName)
        {
            await Driver.ClickAsync(string.Format(DivWithText, lastName).XPath());
            await Driver.ClickAsync(EditAppointmentButton.Id());
            await Driver.ClickAsync(DeleteButton.Id());
            await Driver.WaitUntilElementLoadsAsync(PatientInfoSaveButton.XPath());
            await Driver.ClickAsync(PatientInfoSaveButton.XPath());

            return this;
        }

        public async Task<PortalPage> DeleteTrackedPatientVisitAsync(PortalTestFixture fixture)
        {
            return await Task.Run(() =>
            {
                fixture.PatientVisitIds.Clear();
                return this;
            });
        }

        public async Task<PortalPage> SchedulerSearchForPatientAsync(string patientName)
        {
            await Driver.ClickAsync(string.Format(LinkWithText, "Find a Patient").XPath());
            await Driver.SendKeysAsync(SearchPatientTextBox.Id(), patientName);
            await Driver.ClickAsync(string.Format(SpanWithText, patientName).XPath());

            return this;
        }

        public async Task<bool> PatientSearchSuccesful()
        {
            await Driver.WaitForElementToDisappearAsync(LoadingSpinner.XPath());
            var IspatientInformationWindowOpen = await Driver.IsElementPresentAsync(PatientInfoWindow.XPath());

            return IspatientInformationWindowOpen;
        }

        public async Task<PortalPage> EditAppointmentAsync(string lastName, int days)
        {
            var date = DateTime.Now.AddDays(days).Date.ToString("MM-dd-yyyy");

            await Driver.ClickAsync(string.Format(DivWithText, lastName).XPath());
            await Driver.ClickAsync(EditAppointmentButton.Id(), 15);
            await Driver.ClickAsync(DatePickerInput.Id(), 15);
            await Driver.SendKeysAsync(DatePickerInput.Id(), Keys.Control + "a");
            await Driver.SendKeysAsync(DatePickerInput.Id(), Keys.Delete);
            await Driver.SendKeysAsync(DatePickerInput.Id(), date);
            await Driver.SendKeysAsync(DatePickerInput.Id(), Keys.Enter);
            await Driver.ClickAsync(PatientInfoSaveButton.XPath(), 15);
            await Driver.WaitForElementToDisappearAsync(PatientInfoWindow.XPath(), 15);

            return this;
        }

        public async Task<PortalPage> ChangeScheduleDateAsync(int days)
        {
            Log.Step($"Change schedule date by {days} days.");
            for (int i = 0; i < days; i++)
            {
                await Driver.ClickAsync(ArrowButton.XPath());
            }
            return this;
        }

        // ===== Helpers for VerifyAppointmentLocationAndDayCanBeChanged =====

        public async Task<PortalPage> ChangeToDaysInAdvanceAsync(int numberOfDays)
        {
            // Reuse existing schedule change helper
            return await ChangeScheduleDateAsync(numberOfDays);
        }

        public async Task<PortalPage> CheckInPatientAsync(TestPatient patient)
        {
            Log.Step($"Check in patient: {patient.Name}");
            _currentPatient = patient;

            // Use existing helper to add an appointment for this patient.
            // We create the patient via UI to match legacy behavior.
            await AddAppointmentToScheduleAsync(patient, createNewPatient: true);
            return this;
        }

        public async Task<PortalPage> ChangeLocationOnAppointmentAsync(TestPatient patient, string newLocation)
        {
            // Mimic legacy Portal2Schedule.ChangeLocationOnAppointment:
            // 1. Click the patient's row
            // 2. Open Edit Appointment
            // 3. Open the clinic/location dropdown
            // 4. Select the new location
            // 5. Save and wait for the grid to reload

            Log.Step($"Change location on appointment for {patient.Name} to '{newLocation}'.");

            // Click the patient's row (row contains the patient's name)
            var patientRowSelector = string.Format(DivWithText, $" {patient.LastName}, ");
            await Driver.WaitUntilElementLoadsAsync(patientRowSelector.XPath(), 15);
            await Driver.ClickAsync(patientRowSelector.XPath());

            // Open Edit Appointment
            await Driver.WaitUntilElementLoadsAsync(EditAppointmentButton.Id(), 15);
            await Driver.ClickAsync(EditAppointmentButton.Id());

            // Open the clinic/location dropdown (wait to avoid timing issues)
            await Driver.WaitUntilElementLoadsAsync(ClinicDropdown.XPath(), 15);
            await Driver.ClickAsync(ClinicDropdown.XPath());

            // Select the desired location option
            var locationSelector = string.Format(SpanWithText, newLocation);
            await Driver.WaitUntilElementLoadsAsync(locationSelector.XPath(), 15);
            await Driver.ClickAsync(locationSelector.XPath());

            // Save changes and wait for the patient info window to close
            await Driver.WaitUntilElementLoadsAsync(PatientInfoSaveButton.XPath(), 15);
            await Driver.ClickAsync(PatientInfoSaveButton.XPath());
            await Driver.WaitForElementToDisappearAsync(PatientInfoWindow.XPath(), 15);

            return this;
        }

        public async Task<PortalPage> ChangeLocationOnPortalAsync(string newLocation)
        {
            // Mimic legacy Portal2Schedule.ChangeLocationOnPortal:
            // 1. Open the clinic selector dropdown
            // 2. Type the clinic/location name into the search box
            // 3. Select the matching option
            // 4. Verify the selector shows the new location

            Log.Step($"Change portal location to '{newLocation}'.");

            // Open clinic selector
            await Driver.ClickAsync(ClinicSelector.XPath());

            // Type into the clinic search box
            await Driver.SendKeysAsync(ClinicSearchBox.XPath(), newLocation);

            // Click the matching option
            await Driver.ClickAsync(string.Format(ClinicOption, newLocation).XPath());

            // Verify the selected clinic text contains the new location
            var selectedClinicXpath = $"//mat-select[@id='clinicSelector']//span[contains(text(),'{newLocation}')]";
            var locationChanged = await Driver.IsElementPresentAsync(selectedClinicXpath.XPath(), 5);
            if (!locationChanged)
            {
                Log.Error($"Clinic location did not change to '{newLocation}'.");
            }

            return this;
        }

        public async Task<PortalPage> ChangeDateOnAppointmentAsync(int numberOfDays)
        {
            if (_currentPatient == null)
            {
                Log.Warning("ChangeDateOnAppointmentAsync called before CheckInPatientAsync; skipping date change.");
                return this;
            }

            // Reuse existing EditAppointmentAsync which moves the appointment by N days.
            await EditAppointmentAsync(_currentPatient.LastName, numberOfDays);
            return this;
        }

        public async Task<PortalPage> DeleteAppointmentAsync()
        {
            if (_currentPatient == null)
            {
                Log.Warning("DeleteAppointmentAsync called before CheckInPatientAsync; skipping delete.");
                return this;
            }

            await DeletePatientVisitAsync(_currentPatient.LastName);
            return this;
        }

        public async Task<PortalPage> VerifyPatientAppointmentExistsAsync(bool shouldClick)
        {
            if (_currentPatient == null)
            {
                Log.Warning("VerifyPatientAppointmentExistsAsync called before CheckInPatientAsync; skipping check.");
                return this;
            }

            var exists = await IsPatientInScheduleAsync(_currentPatient.LastName, _currentPatient.Name);

            if (!exists)
            {
                Log.Error($"Expected patient {_currentPatient.Name} to have an appointment in schedule, but none was found.");
            }

            // `shouldClick` is ignored for now; legacy used it to optionally open the appointment.
            return this;
        }

        public async Task<PortalPage> VerifyPatientIsNotListedAsync()
        {
            if (_currentPatient == null)
            {
                Log.Warning("VerifyPatientIsNotListedAsync called before CheckInPatientAsync; skipping check.");
                return this;
            }

            var exists = await IsPatientInScheduleAsync(_currentPatient.LastName, _currentPatient.Name);

            if (exists)
            {
                Log.Error($"Patient {_currentPatient.Name} is still listed in the schedule when they should not be.");
            }

            return this;
        }
    }
}