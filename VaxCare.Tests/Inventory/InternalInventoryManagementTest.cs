using VaxCare.Core;
using VaxCare.Core.Extensions;
using VaxCare.Pages.InternalInventoryManagement;
using VaxCare.Pages.Login;
using Xunit.Abstractions;

namespace VaxCare.Tests.Inventory
{
    public class InternalInventoryManagementTest : BaseTest
    {
        public InternalInventoryManagementTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("https://inventory2stg.vaxcare.com")]
        public async Task SmokeTest(string logInUrl)
        {
            await RunTestAsync("Internal Inventory Management Smoke Test", async () =>
            {
                await PageAsync<InternalInventoryManagementLogin>()
                    .Then(page => page.LoginAsync(logInUrl))
                    .Then(page => page.ClickSidebarMenuHamburger())
                    .Then(page => page.ClickOnPendingOrdersTabAndSwitchToiFrameAndVerifyItLoaded())
                    .Then(page => page.SelectDefaultFrame())
                    .Then(page => page.ClickOnBatchDistTabAndVerifyItLoaded())
                    .Then(page => page.SelectDefaultFrame())
                    .Then(page => page.ClickOnOrderBatchesTabAndVerifyItLoaded())
                    .Then(page => page.SelectDefaultFrame())
                    .Then(page => page.ClickOnOrderHistoryTabAndVerifyItLoaded())
                    .Then(page => page.SelectDefaultFrame())
                    .Then(page => page.ClickOnRunProcessAndVerifyItLoaded())
                    .Then(page => page.SelectDefaultFrame())
                    .Then(page => page.ClickNonSeasonalAutoOrder())
                    .Then(page => page.SelectDefaultFrame())
                    .Then(page => page.ClickOrderabilityManagementAndVerifyItLoaded())
                    .Then(page => page.ClickOrdersAndShipmentsAndVerifyItLoaded())
                    .Then(page => page.SelectDefaultFrame())
                    .Then(page => page.ClickAdjustmentsUploadAndVerifyItLoaded())
                    .Then(page => page.SelectDefaultFrame())
                    .Then(page => page.ClickOnAdjustmentsTabAndVerifyItLoaded())
                    .Then(page => page.SelectDefaultFrame())
                    .Then(page => page.ClickOnCountsAndVerifyItLoaded())
                    .Then(page => page.SelectDefaultFrame())
                    .Then(page => page.ClickBalancesTabAndVerifyItLoaded())
                    .Then(page => page.SelectDefaultFrame())
                    .Then(page => page.ClickBuyBackManagementAndVerifyItLoaded())
                    .Then(page => page.ClickReturnManagementAndVerifyItLoaded())
                    .Then(page => page.ClickOnForecastManagementTabAndVerifyItLoaded());
            });
        }
    }
}

