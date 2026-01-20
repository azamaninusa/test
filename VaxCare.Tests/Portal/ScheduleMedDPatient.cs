using Shouldly;
using VaxCare.Core;
using VaxCare.Core.Extensions;
using VaxCare.Core.TestFixtures;
using VaxCare.Pages.Login;
using Xunit.Abstractions;

namespace VaxCare.Tests.Portal
{
    [Collection("PortalTestCollection")]
    public class ScheduleMedDPatient : BaseTest, IClassFixture<PortalTestFixture>
    {
        private readonly PortalTestFixture _fixture;
        public ScheduleMedDPatient(PortalTestFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("https://identitystg.vaxcare.com", "QaRobot", "MedDEligible", "Schedule a MedD Patient in the Portal")]
        [InlineData("https://identitystg.vaxcare.com", "QaRobot", "PowerRonda", "Schedule a MedD Patient in the Portal")]
        public async Task ScheduleMedDPatientTest(string url, string userName, string testPatient, string testDescription)
        {
            var user = await _fixture.GetUserAsync(userName);
            var (patient, patientRequest) = await _fixture.GetMedDPatientAsync(testPatient);

            await RunTestAsync(testDescription, async () =>
            {
                await _fixture.SetupAsync(Log, user.ClinicId)
                .Then(() => _fixture.AddTestPatientAppointment(patientRequest));

                await PageAsync<PortalLogin>(user)
                .Then(page => page.LoginAsync(url))
                .Then(page => page.WaitForAppointmentGridToLoadAsync())
                .Then(page => page.WaitForEligibilityToRunAsync())
                .Then(page => page.FindPatientInScheduleAsync(patient.LastName))
                .Then(page => page.CheckMedDStatusAsync())
                .Then(page => page.IsMedDCorrectAsync(patient))
                .Then(medDCheck => medDCheck.ShouldBeTrue())
                .Then(() => _fixture.TeardownAsync());
            });
        }
    }
}
