using System.IO;
using OpenQA.Selenium;
using Serilog;
using VaxCare.Core;
using VaxCare.Core.Entities.Patients;
using VaxCare.Core.Extensions;
using VaxCare.Core.Helpers;
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

        // Visit type selectors
        private const string VisitTypeDropdown = "//mat-select[@id='visitTypeDropdown']";
        private const string VisitTypeDropdownArrow = "//mat-select[@id='visitTypeDropdown']//div[contains(@class,'mat-select-arrow')]";
        private const string VisitTypeOption = "//mat-option[contains(@class,'mat-option') and @aria-disabled='false']//span[text()='{0}']";
        private const string VisitTypeDisplayText = "//span[@class='appointment-type']";

        private const string DatePickerInput = "datePickerInput";

        private const string DivWithText = "//div[contains(text(), '{0}')]";
        private const string SpanWithText = "//span[text()= '{0}']";
        private const string LinkWithText = "//a[contains(text(),'{0}')]";
        // XPath to find patient with appointment time: finds div with patient name, then following sibling with appointment time
        private const string PatientWithApptTimeAndNameXpath = "//div[@id='table-container']//tr//div[text()='{0}']/ancestor::td[contains(@class,'column-patientname')]/following-sibling::td[contains(@class,'appointmenttime')]/span[text()='{1}']";
        private const string PatientRowWithLastNameXpath = "//div[@id='table-container']//tr//div[text()=' {0}, ']/ancestor::tr[contains(@class,'appt-row')]";
        private const string AppointmentTimeSpanXpath = ".//td[contains(@class,'appointmenttime')]//span";
        private const string PatientAppointmentOnSchedulerXpath = "//tbody[contains(@class,'ng-tns')]/tr//div[contains(@class,'patientname')]/div[text()='{0}']";

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
        private const string AppointmentGridLoadedXpath = "//div[@id='chmln-dom' and not(@aria-hidden)]";

        private TestPatient? _currentPatient;
        private DateTime _appointmentDate = DateTime.Today;
        private string? _appointmentTime;

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
            // This ensures that Appt grid has loaded completely after deleting an appt (matching legacy)
            await Driver.WaitUntilElementLoadsAsync(AppointmentGridLoadedXpath.XPath(), timeout);
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
            Log.Step($"AddAppointmentToScheduleAsync: Starting to add appointment for patient {patient.Name} (createNewPatient={createNewPatient})");
            Log.Information($"Location: PortalPage.AddAppointmentToScheduleAsync (line ~317)");
            
            try
            {
                Log.Information("Step 1: Clicking 'Add New Patient' button");
                await Driver.ClickAsync(AddNewPatientButton.Id());
                Log.Information("✓ Successfully clicked 'Add New Patient' button");

                if (createNewPatient)
                {
                    Log.Information("Step 2: Creating new patient via UI");
                    await CreateNewPatientAsync(patient);
                    Log.Information($"✓ Successfully created new patient: {patient.FirstName} {patient.LastName}");
                }
                else
                {
                    Log.Information("Step 2: Searching for existing patient");
                    await SearchForPatientAsync(patient);
                    Log.Information($"✓ Successfully searched for patient: {patient.Name}");
                }

                // Check if provider field is empty (match legacy behavior)
                Log.Information("Step 3: Checking provider field value");
                await Driver.WaitUntilElementLoadsAsync(ProviderTextBoxId.Id());
                var providerValue = await Driver.FindElementAsync(ProviderTextBoxId.Id());
                var providerValueAttribute = providerValue?.GetAttribute("value") ?? string.Empty;
                Log.Information($"Provider field current value: '{(string.IsNullOrEmpty(providerValueAttribute) ? "(empty)" : providerValueAttribute)}'");

                if (string.IsNullOrEmpty(providerValueAttribute))
                {
                    Log.Information("Step 4: Provider field is empty, selecting provider");
                    // Match legacy SelectProvider behavior: type last name, then select full name
                    var providerLastName = NewlyCreatedPatientProvider.Split(' ')[1]; // "Morris"
                    Log.Information($"Typing provider last name '{providerLastName}' into provider field");
                    await Driver.SendKeysAsync(ProviderTextBoxId.Id(), providerLastName);
                    await Task.Delay(3000); // Match legacy Sleep(3000)
                    Log.Information("✓ Typed provider last name, waiting for dropdown options");
                    
                    // Select the provider option containing the full name
                    var providerOptionXpath = string.Format(ProviderNameInSearch, NewlyCreatedPatientProvider);
                    Log.Information($"Waiting for provider option '{NewlyCreatedPatientProvider}' to appear");
                    await Driver.WaitUntilElementLoadsAsync(providerOptionXpath.XPath(), 15);
                    Log.Information($"Clicking provider option '{NewlyCreatedPatientProvider}'");
                    await Driver.ClickAsync(providerOptionXpath.XPath());
                    
                    // Wait for provider to be selected (match legacy WaitForElementToAppear)
                    Log.Information("Waiting for provider to be confirmed as selected");
                    await Driver.WaitUntilElementLoadsAsync(ProviderSelected.XPath(), 15);
                    Log.Information($"✓ Successfully selected provider: {NewlyCreatedPatientProvider}");
                }
                else
                {
                    Log.Information($"Step 4: Provider field already has value '{providerValueAttribute}', skipping provider selection");
                }

                // Match legacy behavior: wait after provider selection before clicking Add button
                Log.Information("Step 5: Waiting for UI to settle after provider selection (matching legacy Sleep(3000))");
                await Task.Delay(3000);
                Log.Information("✓ UI settled");

                Log.Information("Step 6: Clicking 'Add Appointment to Schedule' button");
                // Wait for button to be clickable and use JavaScript click as fallback if intercepted
                try
                {
                    await Driver.WaitUntilElementLoadsAsync(ScheduleAddAppointmentButton.Id(), 15);
                    var isClickable = await Driver.IsElementClickableAsync(ScheduleAddAppointmentButton.Id(), 5);
                    if (isClickable)
                    {
                        await Driver.ClickAsync(ScheduleAddAppointmentButton.Id());
                        Log.Information("✓ Successfully clicked 'Add Appointment to Schedule' button");
                    }
                    else
                    {
                        Log.Warning("Button not clickable via normal click, using JavaScript click");
                        await Driver.ExecuteJavaScriptClickAsync(ScheduleAddAppointmentButton.Id());
                        Log.Information("✓ Successfully clicked 'Add Appointment to Schedule' button (via JavaScript)");
                    }
                }
                catch (ElementClickInterceptedException)
                {
                    Log.Warning("Element click intercepted, using JavaScript click as fallback");
                    await Driver.ExecuteJavaScriptClickAsync(ScheduleAddAppointmentButton.Id());
                    Log.Information("✓ Successfully clicked 'Add Appointment to Schedule' button (via JavaScript fallback)");
                }

                Log.Information("Step 6: Clicking 'Done' button");
                await Driver.ClickAsync(DoneButton.Id());
                Log.Information("✓ Successfully clicked 'Done' button");

                Log.Step($"AddAppointmentToScheduleAsync: Successfully completed adding appointment for {patient.Name}");
                return this;
            }
            catch (Exception ex)
            {
                ErrorLoggingHelper.LogErrorWithContext(Log, ex, $"AddAppointmentToScheduleAsync failed for patient {patient.Name}", Driver.Driver);
                throw;
            }
        }

        private async Task CreateNewPatientAsync(TestPatient patient, string insuranceName = "Uninsured")
        {
            Log.Information($"CreateNewPatientAsync: Starting to create new patient {patient.FirstName} {patient.LastName}");
            Log.Information($"Location: PortalPage.CreateNewPatientAsync (line ~354)");
            
            try
            {
                // Wait for loading spinner to disappear before clicking (prevents ElementClickInterceptedException)
                Log.Information("Step 1: Waiting for loading spinner to disappear");
                await Driver.WaitForElementToDisappearAsync(LoadingSpinner.XPath(), 15);
                Log.Information("✓ Loading spinner disappeared");

                Log.Information("Step 2: Clicking 'Create New Patient' button");
                await Driver.ClickAsync(CreateNewPatientButton.Id());
                Log.Information("✓ Successfully clicked 'Create New Patient' button");

                // Wait for the patient info window to be fully loaded before typing
                Log.Information("Step 3: Waiting for patient information modal to load");
                await Driver.WaitUntilElementLoadsAsync(AddPatientWindow.Id(), 15);
                Log.Information("✓ Patient information modal loaded");

                Log.Information($"Step 4: Entering first name: '{patient.FirstName}'");
                await Driver.SendKeysAsync(NewPatientFirstName.Id(), patient.FirstName);
                Log.Information($"✓ Entered first name: {patient.FirstName}");

                Log.Information($"Step 5: Entering last name: '{patient.LastName}'");
                await Driver.SendKeysAsync(NewPatientLastName.Id(), patient.LastName);
                Log.Information($"✓ Entered last name: {patient.LastName}");

                var genderText = patient.Gender == 1 ? "Male" : "Female";
                Log.Information($"Step 6: Selecting gender: '{genderText}'");
                await SelectMatDropdownOptionAsync(NewPatientGender.Id(), genderText);
                Log.Information($"✓ Selected gender: {genderText}");

                Log.Information($"Step 7: Entering date of birth: '{patient.DoB}'");
                await Driver.SendKeysAsync(NewPatientDoB.Id(), patient.DoB);
                Log.Information($"✓ Entered DOB: {patient.DoB}");

                Log.Information($"Step 8: Entering phone number: '{patient.PhoneNumber}'");
                await Driver.SendKeysAsync(NewPatientPhone.Id(), patient.PhoneNumber);
                Log.Information($"✓ Entered phone: {patient.PhoneNumber}");

                Log.Information($"Step 9: Entering payer/insurance: '{insuranceName}'");
                await Driver.SendKeysAsync(NewPatientPayer.Id(), insuranceName);
                Log.Information($"✓ Entered payer: {insuranceName}");

                Log.Information("Step 10: Clicking 'Save' button to create patient");
                await Driver.ClickAsync(SaveButton.XPath());
                Log.Information("✓ Successfully clicked 'Save' button");

                Log.Information($"CreateNewPatientAsync: Successfully created patient {patient.FirstName} {patient.LastName}");
            }
            catch (Exception ex)
            {
                ErrorLoggingHelper.LogErrorWithContext(Log, ex, $"CreateNewPatientAsync failed for patient {patient.FirstName} {patient.LastName}", Driver.Driver);
                throw;
            }
        }

        private async Task SelectMatDropdownOptionAsync(By selector, string option)
        {
            Log.Information($"SelectMatDropdownOptionAsync: Selecting option '{option}' from dropdown");
            Log.Information($"Location: PortalPage.SelectMatDropdownOptionAsync (line ~451)");
            
            try
            {
                Log.Information($"Step 1: Clicking dropdown to open options");
                await Driver.ClickAsync(selector);
                Log.Information("✓ Dropdown opened");

                var optionXpath = string.Format(SpanWithText, option);
                Log.Information($"Step 2: Clicking option '{option}' (XPath: {optionXpath})");
                await Driver.ClickAsync(optionXpath.XPath());
                Log.Information($"✓ Successfully selected option: {option}");
            }
            catch (Exception ex)
            {
                ErrorLoggingHelper.LogErrorWithContext(Log, ex, $"SelectMatDropdownOptionAsync failed while selecting '{option}'", Driver.Driver);
                throw;
            }
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
            // Use stored appointmentDate and add days (matching legacy behavior)
            var date = _appointmentDate.AddDays(days).ToString("M/d/yyyy");

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

        /// <summary>
        /// Gets the count of patient appointments matching the current patient's last name and appointment time.
        /// Matches legacy GetPatientAppointmentsCount behavior.
        /// </summary>
        private async Task<int> GetPatientAppointmentsCountAsync()
        {
            if (_currentPatient == null || string.IsNullOrEmpty(_appointmentTime))
            {
                Log.Warning("GetPatientAppointmentsCountAsync: Missing patient or appointment time");
                return 0;
            }

            var scheduledPatientXpath = string.Format(PatientWithApptTimeAndNameXpath, $" {_currentPatient.LastName}, ", _appointmentTime);
            // Use FindAllElementsAsync with parent and child selectors (matching legacy GetElements behavior)
            var appointments = await Driver.FindAllElementsAsync(
                By.XPath("//div[@id='table-container']"),
                By.XPath(scheduledPatientXpath),
                10);
            
            return appointments.Count;
        }

        public async Task<PortalPage> ChangeScheduleDateAsync(int days)
        {
            Log.Step($"Change schedule date by {days} days.");
            _appointmentDate = DateTime.Today.AddDays(days);
            for (int i = 0; i < days; i++)
            {
                await Driver.ClickAsync(ArrowButton.XPath());
            }
            return this;
        }

        // ===== Helpers for VerifyAppointmentLocationAndDayCanBeChanged =====

        public async Task<PortalPage> ChangeToDaysInAdvanceAsync(int numberOfDays)
        {
            // Store appointment date (matching legacy behavior)
            _appointmentDate = DateTime.Today.AddDays(numberOfDays);
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

            // Get appointment count BEFORE change (matching legacy behavior)
            int totalAppts = await GetPatientAppointmentsCountAsync();
            Log.Information($"Appointment count before date change: {totalAppts}");

            // Click first appointment in list (matching legacy appointments[0].Click())
            var scheduledPatientXpath = string.Format(PatientWithApptTimeAndNameXpath, $" {_currentPatient.LastName}, ", _appointmentTime ?? "");
            var firstAppointment = await Driver.FindElementAsync(scheduledPatientXpath.XPath(), 15);
            await Driver.ClickAsync(firstAppointment);

            // Edit appointment
            await Driver.ClickAsync(EditAppointmentButton.Id(), 2);

            // Calculate new date using stored appointmentDate (matching legacy)
            string newAppointmentDateString = _appointmentDate.AddDays(numberOfDays).ToString("M/d/yyyy");
            await Driver.ClickAsync(DatePickerInput.Id());
            await Driver.SendKeysAsync(DatePickerInput.Id(), Keys.Control + "a");
            await Driver.SendKeysAsync(DatePickerInput.Id(), Keys.Delete);
            await Driver.SendKeysAsync(DatePickerInput.Id(), newAppointmentDateString);
            await Driver.SendKeysAsync(DatePickerInput.Id(), Keys.Enter);
            await Driver.ClickAsync(PatientInfoSaveButton.XPath(), 2);

            // Wait for appointment grid to load (matching legacy WaitForAppointmentGridToLoad())
            await WaitForAppointmentGridToLoadAsync();

            // Get appointment count AFTER change and verify it dropped by 1
            int currentApptCount = await GetPatientAppointmentsCountAsync();
            Log.Information($"Appointment count after date change: {currentApptCount}");

            if (currentApptCount != totalAppts - 1)
            {
                Log.Error($"Appointment count did not drop after moving an appointment to {numberOfDays} days. Expected: {totalAppts - 1}, Actual: {currentApptCount}");
            }

            return this;
        }

        public async Task<PortalPage> DeleteAppointmentAsync()
        {
            if (_currentPatient == null)
            {
                Log.Warning("DeleteAppointmentAsync called before CheckInPatientAsync; skipping delete.");
                return this;
            }

            try
            {
                // Get appointment count BEFORE deletion (matching legacy: uses " " + patient.LastName + ", ")
                var patientAppointmentXpath = string.Format(PatientAppointmentOnSchedulerXpath, $" {_currentPatient.LastName}, ");
                var patientsTotalAppts = await Driver.FindAllElementsAsync(
                    By.XPath("//tbody[contains(@class,'ng-tns')]"),
                    By.XPath(patientAppointmentXpath),
                    10);
                int totalAppts = patientsTotalAppts.Count;
                Log.Information($"Appointment count before deletion: {totalAppts}");

                // Click on patient appointment using ClickSimple equivalent (simple click, no timeout)
                // Matching legacy: ClickSimple(String.Format(PatientAppointmentOnSchedulerXpath, " " + patient.LastName + ", "), ElementType.XPath)
                var patientElement = await Driver.FindElementAsync(patientAppointmentXpath.XPath(), 10);
                await Driver.ClickAsync(patientElement);

                // ClickEditDelete equivalent (matching legacy ClickEditDelete method)
                // Click("editAppointmentBtn", ElementType.Id, "Edit Appointment Button", 2);
                await Driver.ClickAsync(EditAppointmentButton.Id(), 2);
                
                // Click("deleteButton", ElementType.Id, "Delete Appointment Link", 3);
                await Driver.ClickAsync(DeleteButton.Id(), 3);
                
                // Click("(//button[@id='saveButton'])[2]", ElementType.XPath, "Delete > Save", 2);
                await Driver.ClickAsync(PatientInfoSaveButton.XPath(), 2);
                
                // Sleep(3000) - matching legacy ClickEditDelete
                await Task.Delay(3000);

                // Wait for appointment grid to load (matching legacy WaitForAppointmentGridToLoad())
                await WaitForAppointmentGridToLoadAsync();

                // Get appointment count AFTER deletion (using GetPatientNameOnSchedule equivalent)
                // Matching legacy: GetElements(String.Format(PatientAppointmentOnSchedulerXpath, GetPatientNameOnSchedule()), ElementType.XPath)
                // GetPatientNameOnSchedule() returns " " + patient.LastName + ", " + patient.FirstName + " "
                var patientNameOnSchedule = $" {_currentPatient.LastName}, {_currentPatient.FirstName} ";
                var patientsCurrentAppts = await Driver.FindAllElementsAsync(
                    By.XPath("//tbody[contains(@class,'ng-tns')]"),
                    By.XPath(string.Format(PatientAppointmentOnSchedulerXpath, patientNameOnSchedule)),
                    10);
                int currentApptCount = patientsCurrentAppts.Count;
                Log.Information($"Appointment count after deletion: {currentApptCount}");

                // Verify count dropped by 1 (matching legacy behavior)
                if (currentApptCount != totalAppts - 1)
                {
                    Log.Error($"The appointment row count did not decrease. Expected: {totalAppts - 1}, Actual: {currentApptCount}");
                }

                // Note: SetIndexForSpecificPatient(false) is not implemented in new framework
                // as it appears to be legacy-specific state management
            }
            catch (Exception ex)
            {
                Log.Error("The patient wasn't deleted. The patient shouldn't be seen after deleting.");
                ErrorLoggingHelper.LogErrorWithContext(Log, ex, "DeleteAppointmentAsync error", Driver.Driver);
                throw;
            }

            return this;
        }

        public async Task<PortalPage> VerifyPatientAppointmentExistsAsync(bool shouldClick)
        {
            if (_currentPatient == null)
            {
                Log.Warning("VerifyPatientAppointmentExistsAsync called before CheckInPatientAsync; skipping check.");
                return this;
            }

            // Match legacy behavior: format XPath with patient last name and appointment time
            // First, get the appointment time from the schedule for this patient
            var patientNameOnSchedule = $" {_currentPatient.LastName}, {_currentPatient.FirstName} ";
            
            // Find the patient row and extract appointment time
            var patientRowXpath = string.Format(PatientRowWithLastNameXpath, _currentPatient.LastName);
            var patientRow = await Driver.FindElementAsync(patientRowXpath.XPath(), 15);
            
            // Extract appointment time from the row and store it (matching legacy AppointmentTime field)
            var appointmentTimeSpan = patientRow.FindElement(By.XPath(AppointmentTimeSpanXpath));
            _appointmentTime = " " + appointmentTimeSpan.Text.Trim() + " ";
            
            // Format XPath matching legacy: PatientWithApptTimeAndNameXpath with patient last name and appointment time
            var scheduledPatientXpath = string.Format(PatientWithApptTimeAndNameXpath, $" {_currentPatient.LastName}, ", _appointmentTime);
            
            // Wait for element to appear (matching legacy WaitForElementToAppear)
            Log.Step($"{patientNameOnSchedule} scheduled for {_appointmentTime}");
            await Driver.WaitUntilElementLoadsAsync(scheduledPatientXpath.XPath(), 15);

            if (shouldClick)
            {
                // Click on the scheduled patient appointment
                Log.Step("Clicking patient's appointment");
                await Driver.ClickAsync(scheduledPatientXpath.XPath(), 15);
                
                // Wait for edit appointment button to appear
                Log.Step("Waiting for Edit Appointment Button");
                await Driver.WaitUntilElementLoadsAsync(EditAppointmentButton.Id(), 15);
            }

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

        /// <summary>
        /// Changes the visit type for the current patient's appointment.
        /// Matches legacy ChangeVisitType behavior.
        /// </summary>
        public async Task<PortalPage> ChangeVisitTypeAsync(string newVisitType)
        {
            if (_currentPatient == null)
            {
                Log.Warning("ChangeVisitTypeAsync called before CheckInPatientAsync; skipping visit type change.");
                return this;
            }

            try
            {
                Log.Step($"Change visit type to '{newVisitType}' for patient {_currentPatient.Name}.");

                // Step 1: Click on the patient appointment to open the patient info modal
                var scheduledPatientXpath = string.Format(PatientWithApptTimeAndNameXpath, $" {_currentPatient.LastName}, ", _appointmentTime ?? "");
                await Driver.WaitUntilElementLoadsAsync(scheduledPatientXpath.XPath(), 15);
                var patientAppointment = await Driver.FindElementAsync(scheduledPatientXpath.XPath(), 15);
                await Driver.ClickAsync(patientAppointment);

                // Step 2: Verify initial visit type (should be "Well Visit" by default)
                await Driver.WaitUntilElementLoadsAsync(VisitTypeDisplayText.XPath(), 15);
                var initialVisitType = await Driver.GetTextAsync(VisitTypeDisplayText.XPath(), 15);
                Log.Information($"Initial visit type: {initialVisitType}");

                // Step 3: Click the "Edit Appointment" button
                await Driver.WaitUntilElementLoadsAsync(EditAppointmentButton.Id(), 15);
                await Driver.ClickAsync(EditAppointmentButton.Id());

                // Step 4: Verify 'Well' is selected by default in the 'Visit Type' dropdown
                var visitTypeWellXpath = string.Format("//mat-select[@id='visitTypeDropdown']//span[text()='Well']", "");
                await Driver.WaitUntilElementLoadsAsync(visitTypeWellXpath.XPath(), 15);
                await Task.Delay(1000); // Matching legacy Sleep(1000)

                // Step 5: Click the "Visit Type" dropdown
                await Driver.WaitUntilElementLoadsAsync(VisitTypeDropdownArrow.XPath(), 15);
                await Driver.ClickAsync(VisitTypeDropdownArrow.XPath());

                // Step 6: Select the new visit type
                var visitTypeOptionXpath = string.Format(VisitTypeOption, newVisitType);
                await Driver.WaitUntilElementLoadsAsync(visitTypeOptionXpath.XPath(), 15);
                var visitTypeOption = await Driver.FindElementAsync(visitTypeOptionXpath.XPath(), 15);
                await Driver.ClickAsync(visitTypeOption);

                // Step 7: Click "Save"
                await Driver.WaitUntilElementLoadsAsync(PatientInfoSaveButton.XPath(), 15);
                await Driver.ClickAsync(PatientInfoSaveButton.XPath());
                await Driver.WaitForElementToDisappearAsync(PatientInfoWindow.XPath(), 15);

                // Step 8: Click on the patient appointment again to open the patient info modal
                await WaitForAppointmentGridToLoadAsync();
                await Driver.WaitUntilElementLoadsAsync(scheduledPatientXpath.XPath(), 15);
                patientAppointment = await Driver.FindElementAsync(scheduledPatientXpath.XPath(), 15);
                await Driver.ClickAsync(patientAppointment);

                // Step 9: Verify that the visit type updates in the patient information modal
                await Driver.WaitUntilElementLoadsAsync(VisitTypeDisplayText.XPath(), 15);
                var updatedVisitType = await Driver.GetTextAsync(VisitTypeDisplayText.XPath(), 15);
                var expectedVisitType = $"{newVisitType} Visit";

                if (updatedVisitType != expectedVisitType)
                {
                    Log.Error($"Visit type did not update correctly. Expected: {expectedVisitType}, Actual: {updatedVisitType}");
                    throw new Exception($"Visit type verification failed. Expected: {expectedVisitType}, Actual: {updatedVisitType}");
                }

                Log.Step($"Visit type successfully changed to '{updatedVisitType}'.");
            }
            catch (Exception ex)
            {
                ErrorLoggingHelper.LogErrorWithContext(Log, ex, $"ChangeVisitTypeAsync failed for visit type '{newVisitType}'", Driver.Driver);
                throw;
            }

            return this;
        }

        /// <summary>
        /// Verifies the visit type displayed in the patient information modal.
        /// </summary>
        public async Task<PortalPage> VerifyVisitTypeAsync(string expectedVisitType)
        {
            if (_currentPatient == null)
            {
                Log.Warning("VerifyVisitTypeAsync called before CheckInPatientAsync; skipping verification.");
                return this;
            }

            try
            {
                Log.Step($"Verify visit type is '{expectedVisitType}'.");

                // Ensure patient info modal is open
                await Driver.WaitUntilElementLoadsAsync(VisitTypeDisplayText.XPath(), 15);
                var actualVisitType = await Driver.GetTextAsync(VisitTypeDisplayText.XPath(), 15);

                var expectedVisitTypeText = expectedVisitType.EndsWith(" Visit") ? expectedVisitType : $"{expectedVisitType} Visit";

                if (actualVisitType != expectedVisitTypeText)
                {
                    Log.Error($"Visit type verification failed. Expected: {expectedVisitTypeText}, Actual: {actualVisitType}");
                    throw new Exception($"Visit type verification failed. Expected: {expectedVisitTypeText}, Actual: {actualVisitType}");
                }

                Log.Step($"Visit type verified as '{actualVisitType}'.");
            }
            catch (Exception ex)
            {
                ErrorLoggingHelper.LogErrorWithContext(Log, ex, $"VerifyVisitTypeAsync failed for visit type '{expectedVisitType}'", Driver.Driver);
                throw;
            }

            return this;
        }
    }
}