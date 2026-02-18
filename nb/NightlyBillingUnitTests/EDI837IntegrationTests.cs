using FluentValidation;
using NightlyBillingConfigManager;
using NightlyBillingData;
using NightlyBillingData.DbModels;
using NightlyBillingValidators;
using VaxCare.Edi.Contracts.Claims;

namespace NightlyBillingUnitTests
{
    public class EDI837IntegrationTests : CPTCalculator
    {
        private EDI837MessageValidator Validator;
        public ClaimDataContainer? Container;
        public ClaimForm? ClaimForm;
        public ExpectedClaimData? ExpectedClaimData;
        public ProviderOverrideData? ProviderOverrideData;
        private List<ClaimTestScenarios> TestScenarios;
        private List<ExpectedClaimData> ExpectedData;
        private SqlQueries Queries;
        public CptCodeData? CptCodeData;

        public string? FilingRuleToApply = "None";

        [OneTimeSetUp]
        public void Setup()
        {
            string? filingRuleToApply = FilingRuleToApply;

            Container = new ClaimDataContainer();
            Validator = new EDI837MessageValidator();
            ClaimForm = new ClaimForm();
            ExpectedClaimData = new ExpectedClaimData();
            ProviderOverrideData = new ProviderOverrideData();
            CptCodeData = new CptCodeData();

            string connectionString = ConfigManager.GetConnectionString("stg");
            Queries = new SqlQueries(connectionString);

            TestScenarios = Queries.GetClaimTestScenarios(filingRuleToApply) ?? [];
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public async Task RegressionTest837Claims(string ageGroup, string adminScenario, string antigen, string expectedCptCode, int doseCount)
        {
            var testCases = ExtractClaimTestScenarios(ageGroup,adminScenario, antigen, expectedCptCode, doseCount);
            if (!testCases.Any())
            {
                Assert.Warn($"No test case found for {ageGroup} - {adminScenario}.");
            }
            else
            {
                var claimContainer = ExtractClaimContainer(testCases, ageGroup, adminScenario, expectedCptCode, antigen);
                var results = Validator.Validate(claimContainer);

                foreach (var failure in results.Errors)
                {
                    Console.WriteLine($"ClaimId {claimContainer?.ExpectedClaimDataDto?.ConsentFormId} failed validation: {failure.ErrorMessage}");
                }

                Assert.True(results.IsValid);
                Console.WriteLine($"Test run complete for ClaimId {claimContainer?.ExpectedClaimDataDto?.ConsentFormId}.");
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public async Task TestFilingRules(string ageGroup, string adminScenario, string antigen, string expectedCptCode, int doseCount)
        {
            var filingRuleToApply = FilingRuleToApply;
            var testCases = ExtractClaimTestScenarios(ageGroup, adminScenario, antigen, expectedCptCode, doseCount);
            if (!testCases.Any())
            {
                Assert.Warn($"No test case found for {ageGroup} - {adminScenario}, Filing Rule: {filingRuleToApply}.");
            }
            else
            {
                var claimContainer = ExtractClaimContainer(testCases, ageGroup, adminScenario, expectedCptCode, antigen);
                var validator = ValidatorFactory.GetValidator(filingRuleToApply);
                var results = validator.Validate(claimContainer);

                foreach (var failure in results.Errors)
                {
                    Console.WriteLine($"ClaimId {claimContainer?.ExpectedClaimDataDto?.ConsentFormId} failed validation: {failure.ErrorMessage}, Filing Rule: {filingRuleToApply}");
                }

                Assert.True(results.IsValid);
                Console.WriteLine($"Test run complete for ClaimId {claimContainer?.ExpectedClaimDataDto?.ConsentFormId}, Filing Rule: {filingRuleToApply}.");
            }
        }

        public List<ClaimTestScenarios> ExtractClaimTestScenarios(string ageGroup, string adminScenario, string antigen, string expectedCptCode, int doseCount)
        {
            var testScenarios = new List<ClaimTestScenarios>();
            if (antigen == "None")
            {
                return testScenarios = TestScenarios.Where(x => x.AdminScenario == adminScenario && x.AgeGroup == ageGroup && x.DoseCount == doseCount).ToList();
            }
            else
                return testScenarios = TestScenarios.Where(x => x.AdminScenario == adminScenario && x.AgeGroup == ageGroup && x.Antigen == antigen && x.DoseCount == doseCount).ToList();
        }

        public ClaimDataContainer? ExtractClaimContainer(List<ClaimTestScenarios> testCases, string ageGroup, string adminScenario, string expectedCptCode, string antigen)
        {
            ExpectedData = Queries.GetExpectedClaimData(testCases) ?? [];
            var messageOut = Queries.GetMessageOut(testCases)?.FirstOrDefault();

            var extractor = new EDI837MessageExtractor();
            var extractedMessage = extractor.Get837MessageObject(messageOut.MessageData);

            var claimId = messageOut.ConsentFormId;
            var testCase = testCases.Where(x => x.ConsentFormId == claimId).ToList();

            var doseQuantity = testCase.Select(x => x.DoseCount).FirstOrDefault();
            var components = testCase.Select(x => x.TotalComponents).FirstOrDefault();

            string? serviceLineQuantity;
            string? serviceLineCharge;

            expectedCptCode = GetCptCodeOverride(FilingRuleToApply, expectedCptCode, extractedMessage, testCase) ?? string.Empty;

            var cptData = GetStandardCptCodeData(ageGroup, adminScenario, expectedCptCode, doseQuantity, components);
            var cptDataOverride = GetQuantityOverride(FilingRuleToApply, expectedCptCode, testCase);

            if (cptDataOverride.ServiceLineQuantity != null && cptDataOverride.ServiceLineCharge != null)
            {
                serviceLineQuantity = cptDataOverride.ServiceLineQuantity;
                serviceLineCharge = cptDataOverride.ServiceLineCharge;
            }
            else
            {
                serviceLineQuantity = cptData.ServiceLineQuantity;
                serviceLineCharge = cptData.ServiceLineCharge;
            }

            var expectedData = new List<ExpectedClaimData>();
            expectedData = ExpectedData.Where(x => x.ConsentFormId == messageOut.ConsentFormId).ToList();
            ExpectedClaimData? expectedClaimData = expectedData.FirstOrDefault();

            if (expectedClaimData == null)
            {
                Assert.Warn($"Payer Entity not mapped for ClaimId: {claimId}, {ageGroup} - {adminScenario}.");
            }

            var providerOverrides = new List<ProviderOverrideData>();
            providerOverrides = Queries.GetProviderOverrideData(claimId);
            ProviderOverrideData? providerOverrideData = providerOverrides?.FirstOrDefault();

            var payerSwitch = new List<PayerSwitchData>();
            payerSwitch = Queries.GetPayerSwitchData(claimId);
            PayerSwitchData? payerSwitchData = payerSwitch?.FirstOrDefault();

            var actualServices = new List<ClaimableServices>();
            actualServices = extractedMessage.Services;
            var actualClaimableServicesData = actualServices?.Where(x => x.ServiceType == VaxCare.Edi.Contracts.ServiceTypes.Vaccine).ToList();

            var expectedServices = new List<ExpectedClaimableServices>();
            expectedServices = Queries.GetClaimServices(claimId);
            var expectedClaimableServicesData = expectedServices?.Where(x => x.ServiceType == 1).ToList();

            var claimContainer = Container?.GetClaimDataContainer(
                    extractedMessage,
                    expectedClaimData,
                    providerOverrideData,
                    payerSwitchData,
                    actualClaimableServicesData,
                    expectedClaimableServicesData,
                    expectedCptCode,
                    serviceLineCharge,
                    serviceLineQuantity
            );
            return claimContainer;
        }

        public static object[] TestCases =
        {
            //  Adult Administration Scenarios
            new object[] { "Adult", "Single-Component", "None", "90471", 1 },
            new object[] { "Adult", "Multi-Component", "None", "90471", 1 },
            new object[] { "Adult", "Private COVID", "COVID-19", "90480", 1 },
            new object[] { "Adult", "RSV", "RSV", "90471", 1 },
            new object[] { "Adult", "Single-Component", "None", "90471", 2 },
            new object[] { "Adult", "Single-Component", "None", "90472", 2 },
            new object[] { "Adult", "Multi-Component", "None", "90471", 2 },
            new object[] { "Adult", "Multi-Component", "None", "90472", 2 },
            new object[] { "Adult", "Single-Component", "None", "90471", 3 },
            new object[] { "Adult", "Single-Component", "None", "90472", 3 },
            new object[] { "Adult", "Multi-Component", "None", "90471", 3 },
            new object[] { "Adult", "Multi-Component", "None", "90472", 3 },
            //  Pediatric Administration Scenarios
            new object[] { "Pediatric", "Single-Component", "None", "90460", 1 },
            new object[] { "Pediatric", "Multi-Component", "None", "90460", 1 },
            new object[] { "Pediatric", "Multi-Component", "None", "90461", 1 },
            new object[] { "Pediatric", "Private COVID", "COVID-19", "90480", 1 },
            new object[] { "Pediatric", "RSV", "RSV", "96380", 1 },
            new object[] { "Pediatric", "Single-Component", "None", "90460", 2 },
            new object[] { "Pediatric", "Single-Component", "None", "90461", 2 },
            new object[] { "Pediatric", "Multi-Component", "None", "90460", 2 },
            new object[] { "Pediatric", "Multi-Component", "None", "90461", 2 },
            new object[] { "Pediatric", "Single-Component", "None", "90460", 3 },
            new object[] { "Pediatric", "Single-Component", "None", "90461", 3 },
            new object[] { "Pediatric", "Multi-Component", "None", "90460", 3 },
            new object[] { "Pediatric", "Multi-Component", "None", "90461", 3 },
            //  Medicare Administration Scenarios
            new object[] { "Medicare", "Single-Component", "Influenza", "G0008", 1 },
            new object[] { "Medicare", "Single-Component", "PCV 21", "G0009", 1 },
            new object[] { "Medicare", "Single-Component", "PCV20", "G0009", 1 },
            new object[] { "Medicare", "Single-Component", "PPSV23", "G0009", 1 },
            new object[] { "Medicare", "Private COVID", "COVID-19", "90480", 1 },
            new object[] { "Medicare", "Single-Component", "Influenza", "G0008", 2 },
            new object[] { "Medicare", "Single-Component", "PCV 21", "G0009", 2 },
            new object[] { "Medicare", "Single-Component", "PCV20", "G0009", 2 },
            new object[] { "Medicare", "Single-Component", "PPSV23", "G0009", 2 },
            new object[] { "Medicare", "Private COVID", "COVID-19", "90480", 2 },
            //  LARC
            new object[] { "Adult", "LARC - Implant", "Implant", "", 1 },
            new object[] { "Adult", "LARC - Injection", "Injection", "", 1 },
            new object[] { "Adult", "LARC - IUD", "IUD", "", 1 }
        };
    }
}
