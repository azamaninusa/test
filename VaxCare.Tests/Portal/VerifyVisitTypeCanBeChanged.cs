using Shouldly;
using VaxCare.Core;
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
        [InlineData("https://identitystg.vaxcare.com", "QaRobot", "FakePatient", "Verify Visit Type Can Be Changed", "Sick")]
        public async Task PortalVerifyVisitTypeCanBeChanged(string url, string userName, string testPatient, string testDescription, string newVisitType)
        {
            var user = await _fixture.GetUserAsync(userName);
            var patient = await _fixture.GetPatientRequestAsync(testPatient);
            var lastName = patient.NewPatient.LastName;
            var name = patient.NewPatient.FullName;

            await RunTestAsync(testDescription, async () =>
            {
                await _fixture.SetupAsync(Log, user.ClinicId)
                .Then(() => _fixture.AddTestPatientAppointment(patient));

                await PageAsync<PortalLogin>(user)
                .Then(page => page.LoginAsync(url))
                .Then(page => page.WaitForAppointmentGridToLoadAsync())
                .Then(page => page.ChangeVisitTypeForPatientAsync(lastName, newVisitType))
                .Then(page => page.IsPatientInScheduleAsync(lastName, name))
                .Then(patientFound => patientFound.ShouldBeTrue())
                .Then(() => _fixture.TeardownAsync());
            });
        }
    }
}
