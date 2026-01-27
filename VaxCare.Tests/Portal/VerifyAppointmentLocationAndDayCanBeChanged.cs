using Shouldly;
using VaxCare.Core;
using VaxCare.Core.Entities.Patients;
using VaxCare.Core.Extensions;
using VaxCare.Core.TestFixtures;
using VaxCare.Pages.Login;
using Xunit.Abstractions;

namespace VaxCare.Tests.Portal
{
    [Collection("PortalTestCollection")]
    public class VerifyAppointmentLocationAndDayCanBeChanged : BaseTest, IClassFixture<PortalTestFixture>
    {
        private readonly PortalTestFixture _fixture;
        
        public VerifyAppointmentLocationAndDayCanBeChanged(PortalTestFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("https://identitystg.vaxcare.com", "QaRobot", "Verify Appointment Location and Day Can Be Changed")]
        public async Task PortalVerifyAppointmentLocationAndDayCanBeChanged(string url, string userName, string testDescription)
        {
            var user = await _fixture.GetUserAsync(userName);
            var patient = TestPatients.SharonRiskFree();

            await RunTestAsync(testDescription, async () =>
            {
                await _fixture.SetupAsync(Log, user.ClinicId);

                await PageAsync<PortalLogin>(user)
                    .Then(page => page.LoginAsync(url))
                    .Then(page => page.WaitForAppointmentGridToLoadAsync())
                    .Then(page => page.ChangeToDaysInAdvanceAsync(7))
                    .Then(page => page.CheckInPatientAsync(patient))
                    .Then(page => page.ChangeLocationOnAppointmentAsync(patient, "QA Two"))
                    .Then(page => page.VerifyPatientIsNotListedAsync())
                    .Then(page => page.ChangeLocationOnPortalAsync("QA Two"))
                    .Then(page => page.ChangeToDaysInAdvanceAsync(7))
                    .Then(page => page.VerifyPatientAppointmentExistsAsync(false))
                    .Then(page => page.ChangeDateOnAppointmentAsync(1))
                    .Then(page => page.ChangeToDaysInAdvanceAsync(1))
                    .Then(page => page.VerifyPatientAppointmentExistsAsync(false))
                    .Then(page => page.DeleteAppointmentAsync())
                    .Then(() => _fixture.TeardownAsync());
            });
        }
    }
}

