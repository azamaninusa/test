using Shouldly;
using VaxCare.Core;
using VaxCare.Core.Extensions;
using VaxCare.Core.TestFixtures;
using VaxCare.Pages.Login;
using Xunit.Abstractions;

namespace VaxCare.Tests.Portal
{
    [Collection("PortalTestCollection")]
    public class VerifyPatientsInSchedule : BaseTest, IClassFixture<PortalTestFixture>
    {
        private readonly PortalTestFixture _fixture;
        public VerifyPatientsInSchedule(PortalTestFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("https://identitystg.vaxcare.com", "QaRobot", "Verify Patients Exist in Schedule", 3)]
        public async Task VerifyPatientsInScheduleTest(string url, string userName, string test, int patientsToAdd)
        {
            var user = await _fixture.GetUserAsync(userName);

            await RunTestAsync(test, async () =>
            {
                await _fixture.SetupAsync(Log, user.ClinicId)
                .Then(() => _fixture.AddAppointments(patientsToAdd));

                await PageAsync<PortalLogin>(user)
                .Then(page => page.LoginAsync(url))
                .Then(page => page.WaitForAppointmentGridToLoadAsync())
                .Then(page => page.PatientVisitsExistInScheduleAsync())
                .Then(visitsExist => visitsExist.ShouldBeTrue())
                .Then(() => _fixture.TeardownAsync());
            });
        }
    }
}
