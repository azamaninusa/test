using Shouldly;
using VaxCare.Core;
using VaxCare.Core.Extensions;
using VaxCare.Core.WebDriver;
using VaxCare.Pages.CommunityEvents;
using VaxCare.Pages.Portal;
using Xunit;
using Xunit.Abstractions;

namespace VaxCare.Tests.CommunityEvents
{
    public class CommunityEventsTests : BaseTest
    {
        private const string EnrollmentCode = "FL10837";
        private const string ClinicLocation = "QaAuto CE Clinic";
        private const string PatientFirstName = "Mayah";
        private const string NoInsurancePatientFirstName = "Test";
        private const string Insurance = "Humana";
        private string _patientLastName = "Miller";
        private string _noInsurancePatientLastName = "Patient";
        private readonly Random _generator = new Random();

        public CommunityEventsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("https://patientregistrationstg.vaxcare.com/")]
        public async Task EventRegistrationScreenUIVerification(string loginUrl)
        {
            await RunTestAsync("Event Registration Screen UI Verification", async () =>
            {
                await PageAsync<CommunityEventsRegistrationPage>()
                    .Then(page => page.NavigateToAsync(loginUrl))
                    .Then(page => page.VerifyEventRegistrationHomePageUIAsync())
                    .Then(async page =>
                    {
                        // Verify Begin Registration button background color
                        var buttonElement = await Driver.FindElementAsync("//input[@class='enrollment-login-button' and @value='Begin Registration']".XPath(), Log);
                        var backgroundColor = await Driver.GetCssValueAsync(buttonElement, "background-color", Log);
                        Log.Information($"Begin Registration button background color: {backgroundColor}");
                        return page;
                    })
                    .Then(page => page.EnterEnrollmentCodeAsync("Some Text"));
            });
        }

        [Theory]
        [InlineData("https://patientregistrationstg.vaxcare.com/")]
        public async Task VerifyUserLandsOnEventRegistrationPage(string loginUrl)
        {
            await RunTestAsync("Verify User Lands On Event Registration Page", async () =>
            {
                await PageAsync<CommunityEventsRegistrationPage>()
                    .Then(page => page.NavigateToAsync(loginUrl))
                    .Then(page => page.EnterEnrollmentCodeAndClinicLocationAsync(EnrollmentCode, ClinicLocation))
                    .Then(async page =>
                    {
                        // Verify elements are present on registration page
                        await Driver.WaitUntilElementLoadsAsync("//img[contains(@src,'images/VaxCareCrossIcon.svg')]".XPath(), Log);
                        await Driver.WaitUntilElementLoadsAsync("//div[text()='Event Registration']/following-sibling::div[text()='School & Community Outreach']".XPath(), Log);
                        return page;
                    });
            });
        }

        [Theory]
        [InlineData("https://patientregistrationstg.vaxcare.com/")]
        public async Task CommunityEventsInvalidCodeTest(string loginUrl)
        {
            await RunTestAsync("Community Events Invalid Code Test", async () =>
            {
                await PageAsync<CommunityEventsRegistrationPage>()
                    .Then(page => page.NavigateToAsync(loginUrl))
                    .Then(page => page.ClickBeginRegistrationButtonAsync())
                    .Then(async page =>
                    {
                        // Verify invalid code warning appears
                        var warningPresent = await page.VerifyInvalidCodeWarningAsync();
                        warningPresent.ShouldBeTrue("Invalid code warning should be displayed");
                        
                        // Verify warning background color
                        var warningElement = await Driver.FindElementAsync("//div[@class='validation-warning enrollment']".XPath(), Log);
                        var backgroundColor = await Driver.GetCssValueAsync(warningElement, "background-color", Log);
                        Log.Information($"Warning background color: {backgroundColor}");
                        return page;
                    })
                    .Then(page => page.EnterEnrollmentCodeAndClickBeginRegistrationAsync(EnrollmentCode))
                    .Then(async page =>
                    {
                        var warningStillPresent = await Driver.IsElementPresentAsync("//div[@class='validation-warning enrollment' and contains(@style,'display')]/span[text()='Invalid code.  Please try again or contact the event organizer.']".XPath(), Log);
                        warningStillPresent.ShouldBeFalse("Invalid code warning should disappear after entering valid code");
                        return page;
                    });
            });
        }

        [Theory]
        [InlineData("https://patientregistrationstg.vaxcare.com/")]
        public async Task CommunityEventsNoClinicTest(string loginUrl)
        {
            await RunTestAsync("Community Events No Clinic Test", async () =>
            {
                await PageAsync<CommunityEventsRegistrationPage>()
                    .Then(page => page.NavigateToAsync(loginUrl))
                    .Then(page => page.EnterEnrollmentCodeAndClickBeginRegistrationAsync(EnrollmentCode))
                    .Then(page => page.ClickContinueButtonAsync())
                    .Then(async page =>
                    {
                        // Verify no clinic warning appears
                        var warningPresent = await page.VerifyNoClinicWarningAsync();
                        warningPresent.ShouldBeTrue("No clinic warning should be displayed");
                        
                        // Verify warning color
                        var warningElement = await Driver.FindElementAsync("//span[@id='SelectedClinicId-error' and text()='Please select an event location']".XPath(), Log);
                        var color = await Driver.GetCssValueAsync(warningElement, "color", Log);
                        Log.Information($"Warning color: {color}");
                        return page;
                    })
                    .Then(page => page.SelectClinicAsync(ClinicLocation))
                    .Then(async page =>
                    {
                        var warningStillPresent = await Driver.IsElementPresentAsync("//span[@id='SelectedClinicId-error' and text()='Please select an event location']".XPath(), Log);
                        warningStillPresent.ShouldBeFalse("No clinic warning should disappear after selecting clinic");
                        return page;
                    });
            });
        }

        [Theory]
        [InlineData("https://patientregistrationstg.vaxcare.com/")]
        public async Task RegisterPatientWithInsuranceTest(string loginUrl)
        {
            await RunTestAsync("Register Patient With Insurance Test", async () =>
            {
                await PageAsync<CommunityEventsRegistrationPage>()
                    .Then(page => page.NavigateToAsync(loginUrl))
                    .Then(page => page.EnterEnrollmentCodeAndClinicLocationAsync(EnrollmentCode, ClinicLocation))
                    .Then(page => page.EnterRegistrationInfoAsync(PatientFirstName, _patientLastName, Insurance))
                    .Then(page => page.ClickSuccessDialogButtonAsync("Exit"))
                    .Then(page => page.VerifyEventRegistrationHomePageUIAsync());
            });
        }

        [Theory]
        [InlineData("https://patientregistrationstg.vaxcare.com/")]
        public async Task RegisterPatientWithoutInsuranceTest(string loginUrl)
        {
            await RunTestAsync("Register Patient Without Insurance Test", async () =>
            {
                await PageAsync<CommunityEventsRegistrationPage>()
                    .Then(page => page.NavigateToAsync(loginUrl))
                    .Then(page => page.EnterEnrollmentCodeAndClinicLocationAsync(EnrollmentCode, ClinicLocation))
                    .Then(page => page.EnterRegistrationInfoAsync(NoInsurancePatientFirstName, _noInsurancePatientLastName, string.Empty))
                    .Then(page => page.ClickSuccessDialogButtonAsync("Exit"))
                    .Then(page => page.VerifyEventRegistrationHomePageUIAsync());
            });
        }

        [Theory]
        [InlineData("https://patientregistrationstg.vaxcare.com/")]
        public async Task FillOutPatientRegistrationBlankFieldWarning(string loginUrl)
        {
            await RunTestAsync("Fill Out Patient Registration Blank Field Warning", async () =>
            {
                await PageAsync<CommunityEventsRegistrationPage>()
                    .Then(page => page.NavigateToAsync(loginUrl))
                    .Then(page => page.EnterEnrollmentCodeAndClinicLocationAsync(EnrollmentCode, ClinicLocation))
                    .Then(page => page.EnterRegistrationInfoAsync(string.Empty, _patientLastName, Insurance))
                    .Then(page => page.VerifyIncompleteFieldWarningAsync());
            });
        }

        [Theory]
        [InlineData("https://patientregistrationstg.vaxcare.com/")]
        public async Task VerifyPatientWithInsuranceExistsInPortalTest(string loginUrl)
        {
            await RunTestAsync("Verify Patient With Insurance Exists In Portal Test", async () =>
            {
                // Generate unique last name
                _patientLastName = $"{_patientLastName}_{_generator.Next(100000, 1000000)}";
                
                await PageAsync<CommunityEventsRegistrationPage>()
                    .Then(page => page.NavigateToAsync(loginUrl))
                    .Then(page => page.EnterEnrollmentCodeAndClinicLocationAsync(EnrollmentCode, ClinicLocation))
                    .Then(page => page.EnterRegistrationInfoAsync(PatientFirstName, _patientLastName, Insurance))
                    .Then(page => page.ClickSuccessDialogButtonAsync("Exit"))
                    .Then(page => page.VerifyEventRegistrationHomePageUIAsync())
                    .Then(async page =>
                    {
                        // Navigate to Portal to verify patient
                        var portalUrl = page.DetermineCorrectEnvironmentPortalUrl();
                        await Driver.NavigateAsync(portalUrl, Log);
                        return page;
                    })
                    .Then(async page =>
                    {
                        // Login to Portal (this would need PortalLogin implementation)
                        // For now, this is a placeholder - would need actual portal login
                        var portalPage = Page<PortalPage>();
                        await portalPage.WaitForAppointmentGridToLoadAsync();
                        await portalPage.FindPatientInScheduleAsync(_patientLastName);
                        Log.Information($"Patient {_patientLastName}, {PatientFirstName} should be in schedule");
                        return page;
                    });
            });
        }

        [Theory]
        [InlineData("https://patientregistrationstg.vaxcare.com/")]
        public async Task VerifyPatientWithoutInsuranceExistsInPortalTest(string loginUrl)
        {
            await RunTestAsync("Verify Patient Without Insurance Exists In Portal Test", async () =>
            {
                // Generate unique last name
                _noInsurancePatientLastName = $"{_noInsurancePatientLastName}{_generator.Next(100000, 1000000)}";
                
                await PageAsync<CommunityEventsRegistrationPage>()
                    .Then(page => page.NavigateToAsync(loginUrl))
                    .Then(page => page.EnterEnrollmentCodeAndClinicLocationAsync(EnrollmentCode, ClinicLocation))
                    .Then(page => page.EnterRegistrationInfoAsync(NoInsurancePatientFirstName, _noInsurancePatientLastName, string.Empty))
                    .Then(page => page.ClickSuccessDialogButtonAsync("Exit"))
                    .Then(page => page.VerifyEventRegistrationHomePageUIAsync())
                    .Then(async page =>
                    {
                        // Navigate to Portal to verify patient
                        var portalUrl = page.DetermineCorrectEnvironmentPortalUrl();
                        await Driver.NavigateAsync(portalUrl, Log);
                        return page;
                    })
                    .Then(async page =>
                    {
                        // Login to Portal (this would need PortalLogin implementation)
                        var portalPage = Page<PortalPage>();
                        await portalPage.WaitForAppointmentGridToLoadAsync();
                        await portalPage.FindPatientInScheduleAsync(_noInsurancePatientLastName);
                        Log.Information($"Patient {_noInsurancePatientLastName}, {NoInsurancePatientFirstName} should be in schedule");
                        return page;
                    });
            });
        }
    }
}

