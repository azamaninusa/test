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
    public class VerifyVisitTypeCanBeChanged : BaseTest, IClassFixture<PortalTestFixture>
    {
        private readonly PortalTestFixture _fixture;
        
        public VerifyVisitTypeCanBeChanged(PortalTestFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("https://identitystg.vaxcare.com", "QaRobot", "Verify Visit Type Can Be Changed")]
        public async Task PortalVerifyVisitTypeCanBeChanged(string url, string userName, string testDescription)
        {
            var user = await _fixture.GetUserAsync(userName);
            var patient = TestPatients.SharonRiskFree();

            await RunTestAsync(testDescription, async () =>
            {
                await _fixture.SetupAsync(Log, user.ClinicId);

                await PageAsync<PortalLogin>(user)
                    .Then(page => page.LoginAsync(url))
                    .Then(page => page.ChangeToDaysInAdvanceAsync(7))
                    .Then(page => page.CheckInPatientAsync(patient))
                    .Then(page => page.VerifyPatientAppointmentExistsAsync(true))
                    .Then(page => page.ChangeVisitTypeAsync("Sick"))
                    .Then(page => page.DeleteAppointmentAsync())
                    .Then(() => _fixture.TeardownAsync());
            });
        }
    }
}
