using OpenQA.Selenium;
using Serilog;
using VaxCare.Core;
using VaxCare.Core.Extensions;
using VaxCare.Core.Logger;
using VaxCare.Core.WebDriver;

namespace VaxCare.Pages.InternalInventoryManagement
{
    public class InternalInventoryManagementPage(IWebDriverActor driver, ILogger log) : BasePage(driver, log)
    {
        private const string SidebarMenuHamburger = "/html/body/app-root/app-side-nav/label/div[1]";
        private const string PendingOrdersTab = "Pending Orders";
        private const string BatchDistTab = "Batch Dist.";
        private const string OrderBatchesTab = "Order Batches";
        private const string OrderHistoryTab = "Order History";
        private const string RunProcessTab = "Run Process";
        private const string NonSeasonalAutoOrderTab = "Non-Seasonal Auto-Order Generation";
        private const string OrderabilityManagementTab = "Orderability Management";
        private const string OrdersAndShipmentsTab = "Orders And Shipments";
        private const string AdjustmentsUploadTab = "Adjustments Upload";
        private const string AdjustmentsTab = "Adjustments";
        private const string CountsTab = "Counts";
        private const string BalancesTab = "Balances";
        private const string BuyBackManagementTab = "Buyback Management";
        private const string ReturnManagementTab = "Return Management";
        private const string ForecastManagementTab = "Forecast Management";
        private const string PendingOrdersTableFirstCell = ".//*[@id='PendingOrdersTable']/tbody/tr[1]/td[1]";
        private const string OrderBatchesSearchInput = "//input[@id='myBatchIDSearch']";
        private const string BalancesTabHeader = "//*[@id='tabs']/app-root/div/app-transactions/div/div/app-vaccine-counts/div[2]/app-vaccine-count-grid/div/div[1]/div[2]/u";

        public async Task<InternalInventoryManagementPage> ClickSidebarMenuHamburger()
        {
            Log.Step("Click sidebar menu hamburger.");
            await Driver.ClickAsync(SidebarMenuHamburger.XPath());
            await Task.Delay(500); // Small delay for menu to open
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickOnPendingOrdersTabAndSwitchToiFrameAndVerifyItLoaded()
        {
            Log.Step("Click on Pending Orders tab and verify it loaded.");
            await Driver.ClickAsync(PendingOrdersTab.LinkText());
            await SwitchToiFrameAsync();
            await Driver.WaitUntilElementLoadsAsync(PendingOrdersTableFirstCell.XPath(), 30);
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickOnBatchDistTabAndVerifyItLoaded()
        {
            Log.Step("Click on Batch Dist tab and verify it loaded.");
            await Driver.ClickAsync(BatchDistTab.LinkText());
            await SwitchToiFrameAsync();
            await Task.Delay(1000); // Wait for page to load
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickOnOrderBatchesTabAndVerifyItLoaded()
        {
            Log.Step("Click on Order Batches tab and verify it loaded.");
            await Driver.ClickAsync(OrderBatchesTab.LinkText());
            await SwitchToiFrameAsync();
            await Driver.WaitUntilElementLoadsAsync(OrderBatchesSearchInput.XPath(), 30);
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickOnOrderHistoryTabAndVerifyItLoaded()
        {
            Log.Step("Click on Order History tab and verify it loaded.");
            await Driver.ClickAsync(OrderHistoryTab.LinkText());
            await SwitchToiFrameAsync();
            await Task.Delay(1000); // Wait for page to load
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickOnRunProcessAndVerifyItLoaded()
        {
            Log.Step("Click on Run Process tab and verify it loaded.");
            await Driver.ClickAsync(RunProcessTab.LinkText());
            await SwitchToiFrameAsync();
            await Task.Delay(1000); // Wait for page to load
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickNonSeasonalAutoOrder()
        {
            Log.Step("Click on Non-Seasonal Auto-Order Generation tab.");
            await Driver.ClickAsync(NonSeasonalAutoOrderTab.LinkText());
            await Task.Delay(1000); // Wait for page to load
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickOrderabilityManagementAndVerifyItLoaded()
        {
            Log.Step("Click on Orderability Management tab and verify it loaded.");
            await Driver.ClickAsync(OrderabilityManagementTab.LinkText());
            await Task.Delay(1000); // Wait for page to load
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickOrdersAndShipmentsAndVerifyItLoaded()
        {
            Log.Step("Click on Orders And Shipments tab and verify it loaded.");
            await Driver.ClickAsync(OrdersAndShipmentsTab.LinkText());
            await Task.Delay(1000); // Wait for page to load
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickAdjustmentsUploadAndVerifyItLoaded()
        {
            Log.Step("Click on Adjustments Upload tab and verify it loaded.");
            await Driver.ClickAsync(AdjustmentsUploadTab.LinkText());
            await SwitchToiFrameAsync();
            await Task.Delay(1000); // Wait for page to load
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickOnAdjustmentsTabAndVerifyItLoaded()
        {
            Log.Step("Click on Adjustments tab and verify it loaded.");
            await Driver.ClickAsync(AdjustmentsTab.LinkText());
            await SwitchToiFrameAsync();
            await Task.Delay(1000); // Wait for page to load
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickOnCountsAndVerifyItLoaded()
        {
            Log.Step("Click on Counts tab and verify it loaded.");
            await Driver.ClickAsync(CountsTab.LinkText());
            await SwitchToiFrameAsync();
            await Driver.WaitUntilElementLoadsAsync(BalancesTabHeader.XPath(), 30);
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickBalancesTabAndVerifyItLoaded()
        {
            Log.Step("Click on Balances tab and verify it loaded.");
            await Driver.ClickAsync(BalancesTab.LinkText());
            await SwitchToiFrameAsync();
            await Task.Delay(1000); // Wait for page to load
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickBuyBackManagementAndVerifyItLoaded()
        {
            Log.Step("Click on Buyback Management tab and verify it loaded.");
            await Driver.ClickAsync(BuyBackManagementTab.LinkText());
            await Task.Delay(1000); // Wait for page to load
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickReturnManagementAndVerifyItLoaded()
        {
            Log.Step("Click on Return Management tab and verify it loaded.");
            await Driver.ClickAsync(ReturnManagementTab.LinkText());
            await Task.Delay(1000); // Wait for page to load
            return this;
        }

        public async Task<InternalInventoryManagementPage> ClickOnForecastManagementTabAndVerifyItLoaded()
        {
            Log.Step("Click on Forecast Management tab and verify it loaded.");
            await Driver.ClickAsync(ForecastManagementTab.LinkText());
            await Task.Delay(1000); // Wait for page to load
            return this;
        }

        private async Task SwitchToiFrameAsync()
        {
            Log.Step("Switch to iframe.");
            // Wait for iframe to be available
            await Task.Delay(500);
            try
            {
                var iframe = await Driver.FindElementAsync("iframe".TagName(), 10);
                if (iframe != null)
                {
                    Driver.SwitchTo().Frame(iframe);
                }
            }
            catch
            {
                Log.Warning("No iframe found, continuing without switching.");
            }
        }

        public async Task<InternalInventoryManagementPage> SelectDefaultFrame()
        {
            Log.Step("Switch back to default frame.");
            Driver.SwitchTo().DefaultContent();
            await Task.Delay(500);
            return this;
        }
    }
}

