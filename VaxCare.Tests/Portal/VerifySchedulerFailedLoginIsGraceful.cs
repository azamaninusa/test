using Shouldly;
using VaxCare.Core;
using VaxCare.Core.Entities;
using VaxCare.Core.Extensions;
using VaxCare.Pages.Login;
using Xunit.Abstractions;

namespace VaxCare.Tests.Portal
{
    public class VerifySchedulerFailedLoginIsGraceful(ITestOutputHelper output) : BaseTest(output)
    {
        [Theory]
        [InlineData("https://identitystg.vaxcare.com", "Verify Scheduler Failed Login is Graceful")]
        public async Task VerifySchedulerFailedLoginIsGracefulTest(string url, string test)
        {
            await RunTestAsync(test, async () =>
            {
                var user = Users.SecondBaptistChurch();
                await (await PageAsync<PortalLogin>(user)).LoginAsync(url, true)
                .Then(page => page.ConfirmGracefulLoginFailedMessageAsync())
                .Then(gracefulLoginFailure => gracefulLoginFailure.ShouldBeTrue());
            });
        }
    }
}
