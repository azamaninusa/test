using Shouldly;
using VaxCare.Core;
using VaxCare.Core.Extensions;
using VaxCare.Core.TestFixtures;
using VaxCare.Pages.Login;
using Xunit.Abstractions;

namespace VaxCare.Tests.Portal
{
    [Collection("PortalTestCollection")]
    public class VerifyNoPatientsInSchedule : BaseTest, IClassFixture<PortalTestFixture>
    {
        private readonly PortalTestFixture _fixture;
        public VerifyNoPatientsInSchedule(PortalTestFixture fixture, ITestOutputHelper output) : base(output)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("https://identitystg.vaxcare.com", "QaRobot", "Verify Empty Schedule Loads Successfully")]
        public async Task VerifyNoPatientsInScheduleTest(string url, string userName, string testDescription)
        {
            var user = await _fixture.GetUserAsync(userName);

            await RunTestAsync(testDescription, async () =>
            {
                await _fixture.SetupAsync(Log, user.ClinicId);

                await PageAsync<PortalLogin>(user)
                .Then(page => page.LoginAsync(url))
                .Then(page => page.WaitForAppointmentGridToLoadAsync())
                .Then(page => page.IsScheduleEmptyAsync())
                .Then(emptySchedule => emptySchedule.ShouldBeTrue());
            });
        }
    }
}
