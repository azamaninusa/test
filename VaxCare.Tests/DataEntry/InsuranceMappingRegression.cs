using Shouldly;
using VaxCare.Core;
using VaxCare.Core.Extensions;
using VaxCare.Core.TestFixtures;
using VaxCare.Data.Sales.DTO;
using VaxCare.Pages.Login;
using Xunit.Abstractions;

namespace VaxCare.Tests.DataEntry
{
    public class InsuranceMappingRegression : BaseTest, IClassFixture<SalesDbFixture>
    {
        private readonly SalesDbFixture _salesDbFixture;
        public InsuranceMappingRegression(SalesDbFixture salesDbFixture, ITestOutputHelper output) : base(output)
        {
            _salesDbFixture = salesDbFixture;
        }

        [Theory]
        [InlineData("https://dataentrystg.vaxcare.com/", "Insurance Mapping Regression")]
        public async Task InsuranceMappingRegressionTest(string url, string test)
        {
            var expectedResult = new InsuranceMappingDto
            {
                EmrPayerName = "BLUE CROSS/BLUE SHIELD - NASCO",
                PartnerState = "FL",
                InsuranceMappingFilterId = 4,
                InsuranceId = 1000023014,
                PaymentMode = "PartnerBill",
                Date = DateTime.Now.ToString("yyyy-MM-dd")
            };

            await RunTestAsync(test, async () =>
            {
                await _salesDbFixture.SetLogger(Log)
                .Then(() => _salesDbFixture.SetupInsuranceMappingInDbAsync());

                await PageAsync<DataEntryLogin>()
                .Then(page => page.LoginAsync(url))
                .Then(page => page.NavigateToInsuranceMappingPage())
                .Then(page => page.ClickOnInsuranceToMap())
                .Then(page => page.ClickOnInsurancePayerAsync())
                .Then(page => page.ClickOnInsurancePlanIdAsync())
                .Then(page => page.ClickUploadButton())
                .Then(_ => _salesDbFixture.GetInsuranceMappingResultAsync())
                .Then(dbMappingSuccess => dbMappingSuccess.ShouldBe(expectedResult));
            });
        }
    }
}
