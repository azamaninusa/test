using Shouldly;
using VaxCare.Core;
using VaxCare.Core.Extensions;
using VaxCare.Core.TestFixtures;
using VaxCare.Pages.Login;
using Xunit.Abstractions;

namespace VaxCare.Tests.Portal
{
    [Collection("PortalTestCollection")]
    public class ScheduleOnlineEligNotSupportedPatient : BaseTest, IClassFixture<PortalTestFixture>
    {
        private readonly PortalTestFixture _fixture;
        public ScheduleOnlineEligNotSupportedPatient(PortalTestFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("https://identitystg.vaxcare.com", "QaRobot", "OnlineEligNotSupported", "Schedule a Patient in the Portal where Online Elig is not supported")]
        public async Task ScheduleOnlineEligNotSupportedPatientTest(string url, string userName, string testPatient, string testDescription)
        {
            var user = await _fixture.GetUserAsync(userName);
            var patientRequest = await _fixture.GetPatientRequestAsync(testPatient);
            var patient = await _fixture.GetTestPatientAsync(testPatient);

            await RunTestAsync(testDescription, async () =>
            {
                await _fixture.SetupAsync(Log, user.ClinicId)
                .Then(() => _fixture.AddTestPatientAppointment(patientRequest));

                await PageAsync<PortalLogin>(user)
                .Then(page => page.LoginAsync(url))
                .Then(page => page.WaitForAppointmentGridToLoadAsync())
                .Then(page => page.IsPatientEligibilityCorrectAsync(patient))
                .Then(expectedStatus => expectedStatus.ShouldBeTrue())
                .Then(() => _fixture.TeardownAsync());
            });
        }
    }
}
