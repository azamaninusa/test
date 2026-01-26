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
                await Driver.ClickAsync(TestPatientInSchedule);
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
            await Driver.ClickAsync(RunMedDButton.Id());
            await Driver.WaitUntilElementLoadsAsync(MedDCopayCheckWindow.Id(), 20);
            await FillOutCopayFormAsync();
            await Driver.ClickAsync(MedDSavePaymentInfoButton.XPath());
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

        // Additional constants for appointment management
        private const string ForwardButton = "//button[contains(@class,'button-forward')]";
        private const string PatientRowXpath = "//div[text()='{0}']/ancestor::tr";
        private const string ClinicDropdown = "//mat-select[@id='clinicDropDown']//div[contains(@class,'mat-select-arrow')]";
        private const string ClinicSelector = "//mat-select[@id='clinicSelector']//div[contains(@class,'mat-select-arrow-wrapper')]";
        private const string ClinicSearchBox = "//input[@placeholder='Search']";
        private const string ClinicOption = "//span[contains(text(),'{0}')]";
        private const string DatePickerInput = "//input[@id='datePickerInput']";
        private const string PatientWithApptTimeAndNameXpath = "//div[@id='table-container']//tr//div[text()='{0}']/ancestor::td[contains(@class,'column-patientname')]/following-sibling::td[contains(@class,'appointmenttime')]/span[text()='{1}']";
        private const string PatientAppointmentOnSchedulerXpath = "//tbody[contains(@class,'ng-tns')]/tr//div[contains(@class,'patientname')]/div[text()='{0}']";
        private const string AddPatientsToScheduleButton = "add-patients-to-schedule";
        private const string SearchPatientInput = "searchPatientInput";
        private const string PatientDropdownOption = "//div[contains(@id,'cdk-overlay')]//span[@class='patient-name']/span[text()='{0}']";
        private const string AppointmentTimeDropdown = "//mat-select[@placeholder='Time']//span[contains(@class,'ng-star-inserted')]";
        private const string DoneButtonId = "done-action-button";
        private const string ScheduleTableXpath = "//div[contains(@class,'app-sub-container')]";
        private const string EnabledAddApptButton = "//button[@id='add-patients-to-schedule' and not (contains(@class,'disabled'))]";
        private string? _appointmentTime;
        private TestPatient? _currentPatient;

        public async Task<PortalPage> ChangeToDaysInAdvanceAsync(int numberOfDays)
        {
            Log.Step($"Navigate to {numberOfDays} days in advance.");
            await Driver.WaitUntilElementLoadsAsync(ForwardButton.XPath(), Log, 10);
            
            for (int i = 0; i < numberOfDays; i++)
            {
                await Driver.ClickAsync(ForwardButton.XPath());
                await Task.Delay(500); // Small delay between clicks
            }
            
            await Task.Delay(1000); // Wait for grid to update
            await WaitForAppointmentGridToLoadAsync();
            return this;
        }

        public async Task<PortalPage> CheckInPatientAsync(TestPatient patient)
        {
            Log.Step($"Check in patient: {patient.Name}");
            _currentPatient = patient;
            
            // Step 1: Check if patient is already scheduled and delete if needed
            var isAlreadyScheduled = await SetPatientToVariableAndCheckIfTheyAreAlreadyScheduledAsync(patient);
            if (isAlreadyScheduled)
            {
                await DeletePatientIfAlreadyScheduledAsync();
            }
            
            // Step 2: Enter appointment info and check in patient
            await EnterAppointmentInfoAndCheckInPatientAsync(patient.Name);
            
            return this;
        }

        private async Task<bool> SetPatientToVariableAndCheckIfTheyAreAlreadyScheduledAsync(TestPatient patient)
        {
            Log.Step($"Check if patient {patient.Name} is already scheduled");
            _currentPatient = patient;
            
            var selectedIndex = await SetIndexForSpecificPatientAsync(false);
            if (selectedIndex == -1)
            {
                Log.Information($"{patient.Name} does not exist in the schedule table.");
                return false;
            }
            
            return true;
        }

        private async Task<int> SetIndexForSpecificPatientAsync(bool logErrorForFailure = true)
        {
            bool found = false;
            int i = 0;
            int timeoutValue = logErrorForFailure ? 10 : 0;
            int selectedIndex = -1;

            // Wait for scheduler table
            var schedulerTable = await Driver.FindElementAsync(ScheduleTableXpath.XPath(), Log, 5);
            if (schedulerTable == null)
            {
                Log.Error("The Scheduler table wasn't seen.");
                return -1;
            }

            while (!found && i <= timeoutValue)
            {
                try
                {
                    selectedIndex = await LookForPatientInTableAndSetIndexAsync(false);
                    if (selectedIndex != -1)
                    {
                        found = true;
                        break;
                    }
                }
                catch (Exception ex) when (ex is NoSuchElementException || ex is NullReferenceException)
                {
                    if (timeoutValue == 0)
                    {
                        break;
                    }
                }
                
                i++;
                await Task.Delay(1000);
            }

            if (logErrorForFailure && !found)
            {
                Log.Error($"The patient '{_currentPatient?.Name}' wasn't found in the Check In table.");
                return -1;
            }

            return selectedIndex;
        }

        private async Task<int> LookForPatientInTableAndSetIndexAsync(bool failIfNotFound = true)
        {
            if (_currentPatient == null)
            {
                return -1;
            }

            int selectedIndex = 0;
            bool found = false;

            try
            {
                // Get all appointment rows from the scheduler table
                var tableRows = await FindAllElementsByXPathAsync($"{ScheduleTableXpath}{AppointmentRow}");
                
                foreach (var row in tableRows)
                {
                    try
                    {
                        var rowText = row.Text;
                        if (rowText.Contains(_currentPatient.Name))
                        {
                            found = true;
                            break;
                        }
                        selectedIndex++;
                    }
                    catch (StaleElementReferenceException)
                    {
                        Log.Warning("Table changed while searching for patient. Trying again...");
                        return await LookForPatientInTableAndSetIndexAsync(failIfNotFound);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("stale element reference"))
                {
                    Log.Warning("Table changed while searching for patient. Trying again...");
                    return await LookForPatientInTableAndSetIndexAsync(failIfNotFound);
                }
            }

            if (!found && failIfNotFound)
            {
                Log.Error($"Patient {_currentPatient.Name} was not found in the table.");
                return -1;
            }

            return found ? selectedIndex : -1;
        }

        private async Task<PortalPage> DeletePatientIfAlreadyScheduledAsync()
        {
            Log.Step("Delete patient if already scheduled");
            
            if (_currentPatient == null)
            {
                return this;
            }

            int selectedIndex = await SetIndexForSpecificPatientAsync(false);
            
            while (selectedIndex > 0)
            {
                Log.Information($"Patient found. Deleting {_currentPatient.Name} on row {selectedIndex}");
                await DeleteAppointmentAsync();
                selectedIndex = await SetIndexForSpecificPatientAsync(false);
            }

            return this;
        }

        private async Task<PortalPage> EnterAppointmentInfoAndCheckInPatientAsync(string patientName)
        {
            Log.Step($"Enter appointment info and check in patient: {patientName}");
            
            // Click Add Patients button
            await ClickAddPatientsButtonAsync();
            
            // Search for patient to check in
            await SearchForPatientToCheckInAsync(patientName);
            
            // Wait for Add Appointment button to be enabled
            await Driver.WaitUntilElementLoadsAsync(EnabledAddApptButton.XPath(), Log, 10);
            
            // Get the appointment time
            var appointmentTimeXpath = "//mat-select[@placeholder='Time']//span[contains(@class,'ng-star-inserted')]";
            _appointmentTime = await Driver.GetTextAsync(appointmentTimeXpath.XPath(), Log);
            if (!string.IsNullOrEmpty(_appointmentTime))
            {
                _appointmentTime = $" {_appointmentTime.Trim()} ";
            }
            Log.Information($"Selected appointment time: {_appointmentTime}");
            
            // Select provider if not already set
            var providerInput = await Driver.FindElementAsync("provider".Id(), Log, 5);
            if (providerInput != null)
            {
                var providerValue = providerInput.GetAttribute("value");
                if (string.IsNullOrEmpty(providerValue))
                {
                    await SelectProviderAsync("Dean Morris"); // Default provider
                }
            }
            
            // Click Add Appointment button
            await ClickAddAppointmentButtonAsync();
            
            // Verify patient appointment exists
            await VerifyPatientAppointmentExistsAsync(false);
            
            return this;
        }

        private async Task<PortalPage> ClickAddPatientsButtonAsync()
        {
            Log.Step("Click Add Patients button");
            
            // Check if "Add patients" button for empty schedule exists
            var emptyScheduleButton = "add-patients-to-schedule-no-patients";
            var emptyButtonExists = await Driver.IsElementPresentAsync(emptyScheduleButton.Id(), Log, 2);
            
            if (emptyButtonExists)
            {
                await Driver.ClickAsync(emptyScheduleButton.Id());
            }
            else
            {
                await Driver.ClickAsync("add-patients-to-schedule-sub-header".Id());
            }
            
            // Wait for search patient textbox to appear
            await Driver.WaitUntilElementLoadsAsync(SearchPatientInput.Id(), Log, 10);
            
            return this;
        }

        private async Task<PortalPage> SearchForPatientToCheckInAsync(string patientName)
        {
            Log.Step($"Search for patient to check in: {patientName}");
            
            // Type in the patient name
            await Driver.SendKeysAsync(SearchPatientInput.Id(), patientName, Log);
            await Task.Delay(3000);
            
            // Wait for patient dropdown suggestions to appear
            var patientDropdownOverlay = "//div[contains(@id,'cdk-overlay')]";
            await Driver.WaitUntilElementLoadsAsync(patientDropdownOverlay.XPath(), Log, 10);
            
            // Try to find and click the patient suggestion
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    // Try multiple XPath patterns to find patient suggestions
                    var suggestions = await FindAllElementsByXPathAsync($"{patientDropdownOverlay}//span[@class='patient-name']/span");
                    
                    // If original XPath doesn't find suggestions, try alternative patterns
                    if (suggestions.Count == 0)
                    {
                        suggestions = await FindAllElementsByXPathAsync($"{patientDropdownOverlay}//div[contains(@class,'mat-option') or contains(@class,'patient-item')]");
                    }
                    
                    if (suggestions.Count > 0)
                    {
                        // Find the matching patient
                        foreach (var suggestion in suggestions)
                        {
                            try
                            {
                                var suggestionText = suggestion.Text;
                                if (suggestionText.Contains(patientName.Split(',')[0].Trim())) // Match by last name
                                {
                                    await Driver.ClickAsync(suggestion);
                                    await Task.Delay(1000);
                                    return this;
                                }
                            }
                            catch (StaleElementReferenceException)
                            {
                                // Element became stale, continue to next
                                continue;
                            }
                        }
                        
                        // If no exact match, click the first suggestion
                        if (suggestions.Count > 0)
                        {
                            await Driver.ClickAsync(suggestions[0]);
                            await Task.Delay(1000);
                            return this;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning($"Attempt {attempt} to find patient suggestion failed: {ex.Message}");
                    if (attempt < 3)
                    {
                        await Task.Delay(1000);
                    }
                }
            }
            
            Log.Warning($"Could not find patient suggestion for {patientName}, continuing anyway");
            return this;
        }

        private async Task<PortalPage> SelectProviderAsync(string provider)
        {
            Log.Step($"Select provider: {provider}");
            
            // Type in the provider name (use last name part)
            var providerParts = provider.Split(' ');
            var providerSearchText = providerParts.Length > 1 ? providerParts[1] : provider;
            
            await Driver.SendKeysAsync("provider".Id(), providerSearchText, Log);
            await Task.Delay(3000);
            
            // Click on the provider option from dropdown
            var providerOption = $"//div[contains(@id,'cdk-overlay')]//span[contains(text(),'{provider}')]";
            await Driver.ClickAsync(providerOption.XPath());
            await Task.Delay(1000);
            
            return this;
        }

        private async Task<PortalPage> ClickAddAppointmentButtonAsync()
        {
            Log.Step("Click Add Appointment button");
            
            // Click Add Appointment to Schedule button
            await Driver.ClickAsync(AddPatientsToScheduleButton.Id());
            await Task.Delay(3000);
            
            // Click Done button
            await Driver.ClickAsync(DoneButtonId.Id());
            await Task.Delay(3000);
            
            // Wait until patient eligibility is updated
            await WaitUntilPatientEligibilityIsUpdatedAsync();
            
            return this;
        }

        private async Task<PortalPage> WaitUntilPatientEligibilityIsUpdatedAsync()
        {
            Log.Step("Wait until patient eligibility is updated");
            
            // Wait for eligibility updating icon to disappear
            var updatingIcon = "//tr/div[not(contains(@class,'not-overlay'))]";
            await Driver.WaitForElementToDisappearAsync(updatingIcon.XPath(), Log, 60);
            
            // Add 2 seconds after eligibility is updated to ensure selenium can read it in next step
            await Task.Delay(2000);
            
            return this;
        }

        public async Task<PortalPage> ChangeLocationOnAppointmentAsync(TestPatient patient, string newLocation)
        {
            Log.Step($"Change location on appointment for {patient.Name} to {newLocation}");
            
            // Find and click patient row
            var patientRowXpath = string.Format(PatientRowXpath, $" {patient.LastName}, ");
            await Driver.ClickAsync(patientRowXpath.XPath());
            await Task.Delay(500);
            
            // Click Edit Appointment button
            await Driver.ClickAsync(EditAppointmentButton.Id());
            await Task.Delay(1000);
            
            // Click clinic dropdown
            await Driver.ClickAsync(ClinicDropdown.XPath());
            await Task.Delay(500);
            
            // Select new location
            var locationOption = string.Format(ClinicOption, newLocation);
            await Driver.ClickAsync(locationOption.XPath());
            await Task.Delay(500);
            
            // Save
            await SaveAppointmentAsync();
            
            return this;
        }

        public async Task<PortalPage> ChangeLocationOnPortalAsync(string newLocation)
        {
            Log.Step($"Change location on portal to {newLocation}");
            
            // Click clinic selector
            await Driver.ClickAsync(ClinicSelector.XPath());
            await Task.Delay(500);
            
            // Type in search box
            await Driver.SendKeysAsync(ClinicSearchBox.XPath(), newLocation, Log);
            await Task.Delay(500);
            
            // Select location
            var locationOption = string.Format(ClinicOption, newLocation);
            await Driver.ClickAsync(locationOption.XPath());
            await Task.Delay(1000);
            
            // Verify location changed
            var selectedLocationXpath = string.Format("//mat-select[@id='clinicSelector']//span[contains(text(),'{0}')]", newLocation);
            var locationChanged = await Driver.IsElementPresentAsync(selectedLocationXpath.XPath(), Log, 5);
            if (!locationChanged)
            {
                Log.Error($"Clinic location did not change to {newLocation}");
            }
            
            return this;
        }

        public async Task<PortalPage> ChangeDateOnAppointmentAsync(int numberOfDays)
        {
            Log.Step($"Change appointment date by {numberOfDays} days");
            
            if (_currentPatient == null || string.IsNullOrEmpty(_appointmentTime))
            {
                Log.Error("Patient or appointment time is not set. Cannot change appointment date.");
                return this;
            }
            
            // Get current appointment count using patient name format " LastName, "
            var patientName = $" {_currentPatient.LastName}, ";
            var patientXpath = string.Format(PatientAppointmentOnSchedulerXpath, patientName);
            var appointmentsBefore = await FindAllElementsByXPathAsync(patientXpath);
            int countBefore = appointmentsBefore?.Count ?? 0;
            
            // Click on first appointment
            if (appointmentsBefore != null && appointmentsBefore.Count > 0)
            {
                await Driver.ClickAsync(appointmentsBefore[0]);
                await Task.Delay(500);
            }
            
            // Click Edit Appointment button
            await Driver.ClickAsync(EditAppointmentButton.Id());
            await Task.Delay(1000);
            
            // Calculate new date
            var newDate = DateTime.Today.AddDays(numberOfDays).ToString("M/d/yyyy");
            
            // Clear and enter new date
            var dateInput = await Driver.FindElementAsync(DatePickerInput.XPath(), Log);
            dateInput.Clear();
            await Driver.SendKeysAsync(DatePickerInput.XPath(), newDate, Log);
            await Task.Delay(500);
            
            // Save
            await SaveAppointmentAsync();
            
            // Verify appointment count decreased
            await Task.Delay(1000);
            var appointmentsAfter = await FindAllElementsByXPathAsync(patientXpath);
            int countAfter = appointmentsAfter?.Count ?? 0;
            
            if (countAfter != countBefore - 1)
            {
                Log.Error($"Appointment count did not drop after moving appointment. Before: {countBefore}, After: {countAfter}");
            }
            
            return this;
        }

        public async Task<PortalPage> DeleteAppointmentAsync()
        {
            Log.Step("Delete appointment");
            
            if (_currentPatient == null)
            {
                Log.Error("Patient is not set. Cannot delete appointment.");
                return this;
            }
            
            // Get appointments before deletion using patient name format " LastName, "
            var patientName = $" {_currentPatient.LastName}, ";
            var patientXpath = string.Format(PatientAppointmentOnSchedulerXpath, patientName);
            var appointmentsBefore = await FindAllElementsByXPathAsync(patientXpath);
            int countBefore = appointmentsBefore?.Count ?? 0;
            
            if (appointmentsBefore == null || appointmentsBefore.Count == 0)
            {
                Log.Warning("No appointments found to delete");
                return this;
            }
            
            // Click on appointment
            await Driver.ClickAsync(appointmentsBefore[0]);
            await Task.Delay(500);
            
            // Use ClickEditDelete pattern
            await ClickEditDeleteAsync();
            
            // Wait for grid to reload
            await WaitForAppointmentGridToLoadAsync();
            
            // Verify appointment count decreased
            await Task.Delay(1000);
            var appointmentsAfter = await FindAllElementsByXPathAsync(patientXpath);
            int countAfter = appointmentsAfter?.Count ?? 0;
            
            if (countAfter != countBefore - 1)
            {
                Log.Error($"Appointment count did not decrease after deletion. Before: {countBefore}, After: {countAfter}");
            }
            
            return this;
        }

        private async Task<PortalPage> ClickEditDeleteAsync()
        {
            Log.Step("Click Edit and Delete buttons");
            
            // Click Edit Appointment button
            await Driver.ClickAsync(EditAppointmentButton.Id());
            await Task.Delay(2000);
            
            // Click Delete button
            await Driver.ClickAsync(DeleteButton.Id());
            await Task.Delay(3000);
            
            // Click Save button to confirm deletion
            await Driver.ClickAsync(SaveButton.XPath());
            await Task.Delay(3000);
            
            return this;
        }

        public async Task<PortalPage> VerifyPatientAppointmentExistsAsync(bool shouldClick = false)
        {
            Log.Step("Verify patient appointment exists");
            
            if (_currentPatient == null || string.IsNullOrEmpty(_appointmentTime))
            {
                Log.Error("Patient or appointment time is not set. Cannot verify appointment.");
                return this;
            }
            
            // Format: " LastName, " and appointment time
            var patientName = $" {_currentPatient.LastName}, ";
            var appointmentXpath = string.Format(PatientWithApptTimeAndNameXpath, patientName, _appointmentTime.Trim());
            await Driver.WaitUntilElementLoadsAsync(appointmentXpath.XPath(), Log, 15);
            
            if (shouldClick)
            {
                await Driver.ClickAsync(appointmentXpath.XPath());
                await Driver.WaitUntilElementLoadsAsync(EditAppointmentButton.Id(), Log, 15);
            }
            
            return this;
        }

        public async Task<PortalPage> VerifyPatientIsNotListedAsync()
        {
            Log.Step("Verify patient is not listed in schedule");
            
            if (_currentPatient == null)
            {
                Log.Warning("Patient is not set. Skipping verification.");
                return this;
            }
            
            // Format: " LastName, "
            var patientName = $" {_currentPatient.LastName}, ";
            var patientXpath = string.Format(PatientAppointmentOnSchedulerXpath, patientName);
            var patientExists = await Driver.IsElementPresentAsync(patientXpath.XPath(), Log, 5);
            
            if (patientExists)
            {
                Log.Error($"Patient {_currentPatient.Name} is still listed in the schedule");
            }
            
            return this;
        }

        private async Task SaveAppointmentAsync()
        {
            Log.Step("Save appointment");
            await Driver.ClickAsync(SaveButton.XPath());
            await Task.Delay(2000);
            await WaitForAppointmentGridToLoadAsync();
        }

        private async Task<List<IWebElement>> FindAllElementsByXPathAsync(string xpath)
        {
            return await Task.Run(() =>
            {
                return Driver.FindElements(By.XPath(xpath)).ToList();
            });
        }
    }
}
