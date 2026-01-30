using OpenQA.Selenium;
using Serilog;
using VaxCare.Core;
using VaxCare.Core.Extensions;
using VaxCare.Core.Logger;
using VaxCare.Core.WebDriver;

namespace VaxCare.Pages.CommunityEvents
{
    /// <summary>
    /// Community Events Registration Page - Handles patient registration for community events
    /// </summary>
    public class CommunityEventsRegistrationPage(IWebDriverActor driver, ILogger log) : BasePage(driver, log)
    {
        // Selectors
        private const string EnrollmentCodeInput = "PartnerCode";
        private const string BeginRegistrationButton = "//input[@class='enrollment-login-button' and @value='Begin Registration']";
        private const string ContinueButton = "//input[@class='enrollment-login-button' and @value='Continue']";
        private const string ClinicDropdown = "SelectedClinicId";
        private const string FirstNameInput = "firstName";
        private const string LastNameInput = "lastName";
        private const string DobInput = "dob";
        private const string GenderDropdown = "gender";
        private const string EthnicityDropdown = "ethnicity";
        private const string RaceDropdown = "race";
        private const string Address1Input = "ContactInformation_Address1";
        private const string CityInput = "ContactInformation_City";
        private const string StateDropdown = "ContactInformation_State";
        private const string ZipCodeInput = "ContactInformation_ZipCode";
        private const string PhoneInput = "ContactInformation_Phone";
        private const string UninsuredCheckbox = "//input[@id='uninsured']";
        private const string PrimaryInsuranceInput = "Insurance.PrimaryInsuranceId_input";
        private const string MemberIdInput = "Insurance_MemberId";
        private const string RelationshipToInsuredDropdown = "relationshipToInsured";
        private const string FormSubmitButton = "//input[@id='form-submit' and @value='Submit Registration']";
        private const string AuthorizationSignatureInput = "Authorization_Signature";
        private const string AuthorizationConsentCheckbox = "Authorization_Consent";

        // Validation selectors
        private const string InvalidCodeWarning = "//div[@class='validation-warning enrollment' and contains(@style,'display')]/span[text()='Invalid code.  Please try again or contact the event organizer.']";
        private const string ValidationWarningEnrollmentDiv = "//div[@class='validation-warning enrollment']";
        private const string NoClinicWarning = "//span[@id='SelectedClinicId-error' and text()='Please select an event location']";
        private const string IncompleteFieldWarning = "//div[@class='validation-warning' and @style='display: block;']/span[text()='This form must be completed in order to receive vaccinations.']";
        private const string SuccessModalTitle = "//div[@class='modal-title' and text()='Success!']";
        private const string SuccessModalExitButton = "//button[text()='Exit']";
        private const string SuccessModalSavePrintButton = "//button[@id='print-page' and text()='Save / Print']";

        // UI Elements
        private const string VaxCareLogo = "//img[contains(@src,'images/VaxCareCrossIcon.svg')]";
        private const string EventRegistrationTitle = "//div[text()='Event Registration']/following-sibling::div[text()='School & Community Outreach']";
        private const string GetStartedHeader = "//div[@class='enrollment-login-header']/span[contains(text(),'Get Started')]";
        private const string EnrollmentCodeLabel = "//label[@for='PartnerCode' and text()='Enrollment Code']";

        /// <summary>
        /// Navigates to the Community Events registration page and waits for it to load
        /// </summary>
        /// <param name="url">The URL of the Community Events registration page</param>
        /// <returns>The current page instance for fluent chaining</returns>
        public async Task<CommunityEventsRegistrationPage> NavigateToAsync(string url)
        {
            Log.Step($"Navigate to Community Events registration page: {url}");
            await Driver.NavigateAsync(url);
            
            // Wait for page to load by waiting for a key element (enrollment code input)
            // This ensures the page is fully rendered before proceeding, preventing blank screenshots
            await Driver.WaitUntilElementLoadsAsync(EnrollmentCodeInput.Id(), timeoutInSeconds: 15);
            
            return this;
        }

        public async Task<CommunityEventsRegistrationPage> VerifyEventRegistrationHomePageUIAsync()
        {
            Log.Step("Verify Event Registration Home Page UI elements");
            
            await Driver.WaitUntilElementLoadsAsync(VaxCareLogo.XPath());
            await Driver.WaitUntilElementLoadsAsync(EventRegistrationTitle.XPath());
            await Driver.WaitUntilElementLoadsAsync(GetStartedHeader.XPath());
            await Driver.WaitUntilElementLoadsAsync(EnrollmentCodeLabel.XPath());
            await Driver.WaitUntilElementLoadsAsync($"{EnrollmentCodeInput}".Id());
            
            return this;
        }

        public async Task<CommunityEventsRegistrationPage> EnterEnrollmentCodeAsync(string enrollmentCode)
        {
            Log.Step($"Enter enrollment code: {enrollmentCode}");
            await Driver.SendKeysAsync(EnrollmentCodeInput.Id(), enrollmentCode);
            return this;
        }

        public async Task<CommunityEventsRegistrationPage> ClickBeginRegistrationButtonAsync()
        {
            Log.Step("Click Begin Registration button");
            await Driver.ClickAsync(BeginRegistrationButton.XPath());
            return this;
        }

        public async Task<CommunityEventsRegistrationPage> EnterEnrollmentCodeAndClickBeginRegistrationAsync(string enrollmentCode)
        {
            Log.Step($"Enter enrollment code and click Begin Registration: {enrollmentCode}");
            await EnterEnrollmentCodeAsync(enrollmentCode);
            await ClickBeginRegistrationButtonAsync();
            
            // Verify location dropdown appears
            await Driver.WaitUntilElementLoadsAsync("//label[text()='Location']/following-sibling::select[not(@aria-describedby)]/option[text()='   -- Select --   ']".XPath());
            await Driver.WaitUntilElementLoadsAsync(ContinueButton.XPath());
            
            return this;
        }

        public async Task<CommunityEventsRegistrationPage> SelectClinicAsync(string clinicLocation)
        {
            Log.Step($"Select clinic location: {clinicLocation}");
            await Driver.SelectDropDownOptionByTextAsync(ClinicDropdown.Id(), clinicLocation);
            return this;
        }

        public async Task<CommunityEventsRegistrationPage> ClickContinueButtonAsync()
        {
            Log.Step("Click Continue button");
            await Driver.ClickAsync(ContinueButton.XPath());
            return this;
        }

        public async Task<CommunityEventsRegistrationPage> SelectClinicAndClickContinueAsync(string clinicLocation)
        {
            Log.Step($"Select clinic and click Continue: {clinicLocation}");
            await SelectClinicAsync(clinicLocation);
            await ClickContinueButtonAsync();
            
            // Wait for registration page to load
            await Driver.WaitUntilElementLoadsAsync("//div[@class='enrollment-header' and text()='Event Registration']".XPath());
            
            return this;
        }

        public async Task<CommunityEventsRegistrationPage> EnterEnrollmentCodeAndClinicLocationAsync(string enrollmentCode, string clinicLocation)
        {
            Log.Step($"Enter enrollment code and select clinic: {enrollmentCode}, {clinicLocation}");
            await EnterEnrollmentCodeAndClickBeginRegistrationAsync(enrollmentCode);
            await SelectClinicAndClickContinueAsync(clinicLocation);
            return this;
        }

        public async Task<CommunityEventsRegistrationPage> EnterRegistrationInfoAsync(string firstName, string lastName, string insurance)
        {
            Log.Step($"Enter patient registration information: {firstName} {lastName}, Insurance: {(string.IsNullOrEmpty(insurance) ? "None" : insurance)}");
            
            // Basic Information
            await Driver.SendKeysAsync(FirstNameInput.Id(), firstName);
            await Driver.SendKeysAsync(LastNameInput.Id(), lastName);
            await Driver.SendKeysAsync(DobInput.Id(), "09/28/2018");
            await Driver.SelectDropDownOptionByTextAsync(GenderDropdown.Id(), "Female");
            await Driver.SelectDropDownOptionByTextAsync(EthnicityDropdown.Id(), "Unspecified");
            await Driver.SelectDropDownOptionByTextAsync(RaceDropdown.Id(), "Unspecified");
            
            // Contact Information
            await Driver.SendKeysAsync(Address1Input.Id(), "5 MY STREET");
            await Driver.SendKeysAsync(CityInput.Id(), "ORLANDO");
            await Driver.SelectDropDownOptionByTextAsync(StateDropdown.Id(), "Florida");
            await Driver.SendKeysAsync(ZipCodeInput.Id(), "32806");
            await Driver.SendKeysAsync(PhoneInput.Id(), "382-103-9728");
            
            // Insurance Information
            if (string.IsNullOrEmpty(insurance))
            {
                Log.Step("Patient has no insurance - selecting uninsured option");
                // Verify insurance fields are initially visible
                var hiddenInsuranceContainer = "//div[@class='insured-container' and @style='display: none;']";
                var hiddenPrimaryInsuranceInput = "//input[@name='Insurance.PrimaryInsuranceId_input' and @disabled]";
                
                // Click uninsured checkbox
                await Driver.ClickAsync(UninsuredCheckbox.XPath());
                await Task.Delay(2000); // Wait for UI to update
                
                // Verify insurance fields are now hidden
                await Driver.VerifyElementsPresentOnPageAsync(hiddenInsuranceContainer, hiddenPrimaryInsuranceInput);
            }
            else
            {
                Log.Step($"Enter insurance information: {insurance}");
                await Driver.SendKeysAsync(By.Name(PrimaryInsuranceInput), insurance);
                await Driver.SendKeysAsync(MemberIdInput.Id(), "10742845GBHZ");
                await Driver.SendKeysAsync(PhoneInput.Id(), "382-103-9728"); // Legacy: re-type phone in insurance branch
                await Driver.SelectDropDownOptionByTextAsync(RelationshipToInsuredDropdown.Id(), "Self");
            }
            
            // Authorization and Consent (legacy uses WaitForElementToBeUsable + Sleep between clicks for reliable radio selection)
            Log.Step("Fill out authorization and consent questions");
            await Task.Delay(500); // Allow authorization section to be ready before first radio ("Are you sick today")
            await ClickAuthorizationRadioAsync("//label[@for='Authorization_PatientIsSick_N']");
            await ClickAuthorizationRadioAsync("//label[@for='Authorization_HasAllergy_N']");
            await ClickAuthorizationRadioAsync("//label[@for='Authorization_HadVaccineReaction_N']");
            await ClickAuthorizationRadioAsync("//label[@for='Authorization_HasHealthProblems_N']");
            await ClickAuthorizationRadioAsync("//label[@for='Authorization_Immunocompromised_N']");
            await ClickAuthorizationRadioAsync("//label[@for='Authorization_HadCancerTreatments_N']");
            await ClickAuthorizationRadioAsync("//label[@for='Authorization_HasHadNeurologicalDisease_N']");
            await ClickAuthorizationRadioAsync("//label[@for='Authorization_DoesSmoke_N']");
            await ClickAuthorizationRadioAsync("//label[@for='Authorization_HasHadBloodTreatment_N']");
            await ClickAuthorizationRadioAsync("//label[@for='Authorization_IsPregnant_N']");
            await ClickAuthorizationRadioAsync("//label[@for='Authorization_HadRecentVaccinations_N']");
            await ClickAuthorizationRadioAsync("//label[@for='Authorization_HadInfluenzaVaccination_N']");
            await Driver.ClickAsync(AuthorizationConsentCheckbox.Id());
            await Task.Delay(250);
            
            // Signature
            await Driver.SendKeysAsync(AuthorizationSignatureInput.Id(), "Test Patient");
            
            // Submit Registration
            Log.Step("Submit registration form");
            await Driver.ClickAsync(FormSubmitButton.XPath());
            
            return this;
        }

        public async Task<CommunityEventsRegistrationPage> VerifyIncompleteFieldWarningAsync()
        {
            Log.Step("Verify incomplete field warning is displayed");
            
            // Verify warning message
            await Driver.WaitUntilElementLoadsAsync(IncompleteFieldWarning.XPath());
            
            // Verify first name field has validation error
            await Driver.WaitUntilElementLoadsAsync("//input[@id='firstName']/parent::div[contains(@class,'input-validation-error')]".XPath());
            
            return this;
        }

        public async Task<CommunityEventsRegistrationPage> ClickSuccessDialogButtonAsync(string buttonText)
        {
            Log.Step($"Click {buttonText} button on success dialog");
            
            // Wait for success modal
            await Driver.WaitUntilElementLoadsAsync(SuccessModalTitle.XPath());
            
            // Verify modal elements
            await Driver.WaitUntilElementLoadsAsync("//div[@class='modal-body']/div[text()='Save or print a copy of this form for your records.']".XPath());
            await Driver.WaitUntilElementLoadsAsync(SuccessModalSavePrintButton.XPath());
            await Driver.WaitUntilElementLoadsAsync(SuccessModalExitButton.XPath());
            
            // Click the specified button
            var buttonSelector = $"//button[text()='{buttonText}']";
            await Driver.ClickAsync(buttonSelector.XPath());
            
            return this;
        }

        public async Task<bool> VerifyInvalidCodeWarningAsync()
        {
            Log.Step("Verify invalid code warning is displayed");
            return await Driver.IsElementPresentAsync(InvalidCodeWarning.XPath());
        }

        public async Task<bool> IsInvalidCodeWarningDisplayedAsync()
        {
            return await Driver.IsElementPresentAsync(InvalidCodeWarning.XPath());
        }

        public async Task<string> GetBeginRegistrationButtonBackgroundColorAsync()
        {
            var buttonElement = await Driver.FindElementAsync(BeginRegistrationButton.XPath());
            return await Driver.GetCssValueAsync(buttonElement, "background-color");
        }

        public async Task<CommunityEventsRegistrationPage> WaitForRegistrationPageElementsAsync()
        {
            await Driver.WaitUntilElementLoadsAsync(VaxCareLogo.XPath());
            await Driver.WaitUntilElementLoadsAsync(EventRegistrationTitle.XPath());
            return this;
        }

        public async Task<string> GetInvalidCodeWarningBackgroundColorAsync()
        {
            var warningElement = await Driver.FindElementAsync(ValidationWarningEnrollmentDiv.XPath());
            return await Driver.GetCssValueAsync(warningElement, "background-color");
        }

        public async Task<string> GetNoClinicWarningColorAsync()
        {
            var warningElement = await Driver.FindElementAsync(NoClinicWarning.XPath());
            return await Driver.GetCssValueAsync(warningElement, "color");
        }

        public async Task<bool> IsNoClinicWarningDisplayedAsync()
        {
            return await Driver.IsElementPresentAsync(NoClinicWarning.XPath());
        }

        public async Task<bool> VerifyNoClinicWarningAsync()
        {
            Log.Step("Verify no clinic selected warning is displayed");
            return await Driver.IsElementPresentAsync(NoClinicWarning.XPath());
        }

        /// <summary>
        /// Clicks an authorization radio (label) with a short delay after click so the selection registers before the next radio.
        /// Legacy uses WaitForElementToBeUsable + Sleep between each Click for reliable radio selection.
        /// </summary>
        private async Task ClickAuthorizationRadioAsync(string labelXPath)
        {
            await Driver.ClickAsync(labelXPath.XPath());
            await Task.Delay(250);
        }

        public string DetermineCorrectEnvironmentPortalUrl()
        {
            // This would need to be configured based on environment
            // For now, defaulting to staging
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Staging";
            
            return environment.ToLower() switch
            {
                "development" => "https://identitydev.vaxcare.com/",
                "qa" => "https://identityqa.vaxcare.com/",
                "staging" => "https://identitystg.vaxcare.com/",
                "production" => "https://identity.vaxcare.com/",
                _ => "https://identitystg.vaxcare.com/"
            };
        }
    }
}

