using VaxCare.Edi.Contracts.Claims;
using static NightlyBillingData.DbModels.ServicesDataContainer;

namespace NightlyBillingData.DbModels
{
    public class ClaimDataContainer
    {
        public ClaimForm? ActualClaimDataDto { get; set; }
        public ExpectedClaimData? ExpectedClaimDataDto { get; set; }
        public ProviderOverrideData? ProviderOverrideDataDto { get; set; }
        public PayerSwitchData? PayerSwitchDataDto { get; set; }
        public string? ExpectedCptCode { get; set; }
        public string? ExpectedServiceLineCharge { get; set; }
        public string? ExpectedQuantity { get; set; }
        public List<ServiceData>? ServiceDataDto { get; set; }

        public ClaimDataContainer GetClaimDataContainer(ClaimForm claimForm, ExpectedClaimData expectedClaimData, ProviderOverrideData providerOverrideData, PayerSwitchData payerSwitchData,
            List<ClaimableServices> actual, List<ExpectedClaimableServices> expected, string expectedCptCode, string expectedServiceLineCharge, string expectedQuantity)
        {
            var actualClaimDataDto = claimForm;
            var expectedClaimDataDto = expectedClaimData;
            var providerOverrideDataDto = MapProviderOverrideData(providerOverrideData, expectedClaimData);
            var payerSwitchDataDto = MapPayerSwitchData(payerSwitchData, expectedClaimData);
            var expectedCptCodeDto = expectedCptCode;
            var expectedServiceLineChargeDto = expectedServiceLineCharge;
            var expectedQuantityDto = expectedQuantity;
            var serviceDataDto = MapServiceData(actual, expected);

            var claimDataContainer = new ClaimDataContainer
            {
                ActualClaimDataDto = actualClaimDataDto,
                ExpectedClaimDataDto = expectedClaimDataDto,
                ProviderOverrideDataDto = providerOverrideDataDto,
                PayerSwitchDataDto = payerSwitchDataDto,
                ExpectedCptCode = expectedCptCodeDto,
                ExpectedServiceLineCharge = expectedServiceLineChargeDto,
                ExpectedQuantity = expectedQuantityDto,
                ServiceDataDto = serviceDataDto
            };
            return claimDataContainer;
        }

        public List<ServiceData> MapServiceData(List<ClaimableServices> actual, List<ExpectedClaimableServices> expected)
        {
            var serviceData = (from cpts in actual
                       join ecpts in expected on cpts.CptCodeName equals ecpts.CptCodeName
                       select new ServiceData
                       {
                           CptCode = cpts.CptCodeName,
                           ExpectedNdc = ecpts.Ndc,
                           ActualNdc = cpts.Ndc,
                           ExpectedRxNumber = ExpectedClaimDataDto?.ConsentFormId.ToString(),
                           ActualRxNumber = cpts.RxNumber,
                           ExpectedPrimaryDiagnosisCode = "Z23",
                           ActualPrimaryDiagnosisCode = cpts.DiagCode
                       }).ToList();

            return serviceData;
        }

        public ProviderOverrideData MapProviderOverrideData(ProviderOverrideData providerOverrideData, ExpectedClaimData expectedClaimData)
        {
            var providerOverrideMap = new ProviderOverrideData
            {
                ConsentFormId = providerOverrideData.ConsentFormId,
                PartnerId = providerOverrideData.PartnerId,
                ClinicId = providerOverrideData.ClinicId,
                PayerEntityId = providerOverrideData.PayerEntityId,
                ProviderId = providerOverrideData.ProviderId,
                EicId = providerOverrideData.EicId,
                InformationEntityId = providerOverrideData.InformationEntityId,
                EntityAddressId = providerOverrideData.EntityAddressId,
                RenderingNPISwitchNPI = providerOverrideData.RenderingNPISwitchNPI ?? expectedClaimData.RenderingProviderNpi,
                RenderingNPISwitchFirstName = providerOverrideData.RenderingNPISwitchFirstName ?? expectedClaimData.RenderingProviderFirstName,
                RenderingNPISwitchLastName = providerOverrideData.RenderingNPISwitchLastName ?? expectedClaimData.RenderingProviderLastName,
                FacilityRenderingProviderNPI = providerOverrideData.FacilityRenderingProviderNPI ?? expectedClaimData.RenderingProviderNpi,
                FacilityRenderingProviderFirstName = providerOverrideData.FacilityRenderingProviderFirstName ?? expectedClaimData.RenderingProviderFirstName,
                FacilityRenderingProviderLastName = providerOverrideData.FacilityRenderingProviderLastName ?? expectedClaimData.RenderingProviderLastName,
                BillingProviderOverrideNPI = providerOverrideData.BillingProviderOverrideNPI ?? expectedClaimData.BillingProviderNpi,
                BillingProviderOverrideEIN = providerOverrideData.BillingProviderOverrideEIN ?? expectedClaimData.BillingProviderEin,
                BillingProviderOverrideLastName = providerOverrideData.BillingProviderOverrideLastName ?? expectedClaimData.BillingProviderName,
                BillingProviderOverrideAddressStreet = providerOverrideData.BillingProviderOverrideAddressStreet ?? expectedClaimData.BillingProviderAddressStreet,
                BillingProviderOverrideCity = providerOverrideData.BillingProviderOverrideCity ?? expectedClaimData.BillingProviderAddressCity,
                BillingProviderOverrideState = providerOverrideData.BillingProviderOverrideState ?? expectedClaimData.BillingProviderAddressState,
                BillingProviderOverrideZipCode = providerOverrideData.BillingProviderOverrideZipCode ?? expectedClaimData.BillingProviderAddressZipCode,
                RenderingProviderOverrideNPI = providerOverrideData.RenderingProviderOverrideNPI ?? expectedClaimData.RenderingProviderNpi,
                RenderingProviderOverrideFirstName = providerOverrideData.RenderingProviderOverrideFirstName ?? expectedClaimData.RenderingProviderFirstName,
                RenderingProviderOverrideLastName = providerOverrideData.RenderingProviderOverrideLastName ?? expectedClaimData.RenderingProviderLastName,
                ServicingProviderOverrideNPI = providerOverrideData.ServicingProviderOverrideNPI ?? expectedClaimData.ServicingProviderNpi,
                ServicingProviderOverrideLastName = providerOverrideData.ServicingProviderOverrideLastName ?? expectedClaimData.ServicingProviderName,
                ServicingProviderOverrideAddressStreet = providerOverrideData.ServicingProviderOverrideAddressStreet ?? expectedClaimData.ServicingProviderAddressStreet,
                ServicingProviderOverrideCity = providerOverrideData.ServicingProviderOverrideCity ?? expectedClaimData.ServicingProviderAddressCity,
                ServicingProviderOverrideState = providerOverrideData.ServicingProviderOverrideState ?? expectedClaimData.ServicingProviderAddressState,
                ServicingProviderOverrideZipCode = providerOverrideData.ServicingProviderOverrideZipCode ?? expectedClaimData.ServicingProviderAddressZipCode,
                BillingProviderTaxonomyCode = providerOverrideData.BillingProviderTaxonomyCode ?? expectedClaimData.BillingProviderTaxonomyCode,
                RenderingProviderTaxonomyCode = providerOverrideData.RenderingProviderTaxonomyCode ?? expectedClaimData.RenderingProviderTaxonomyCode
            };
            return providerOverrideMap;
        }

        public PayerSwitchData MapPayerSwitchData(PayerSwitchData payerSwitchData, ExpectedClaimData expectedClaimData)
        {
            var payerSwitchMap = new PayerSwitchData
            {
                ConsentFormId = payerSwitchData.ConsentFormId,
                PartnerId = payerSwitchData.PartnerId,
                ClinicId = payerSwitchData.ClinicId,
                PayerSwitchMapFromPayerEntityId = payerSwitchData.PayerSwitchMapFromPayerEntityId,
                PayerSwitchMapToPayerEntityId = payerSwitchData.PayerSwitchMapToPayerEntityId,
                PayerSwitchMapToPayerName = payerSwitchData.PayerSwitchMapToPayerName ?? expectedClaimData.PrimaryInsuranceName,
                BillingProviderName = payerSwitchData.BillingProviderName ?? expectedClaimData.BillingProviderName,
                BillingProviderNPI = payerSwitchData.BillingProviderNPI ?? expectedClaimData.BillingProviderNpi,
                BillingProviderEIN = payerSwitchData.BillingProviderEIN ?? expectedClaimData.BillingProviderEin,
                BillingProviderTaxonomyCode = payerSwitchData.BillingProviderTaxonomyCode ?? expectedClaimData.BillingProviderTaxonomyCode,
                BillingProviderAddressStreet = payerSwitchData.BillingProviderAddressStreet ?? expectedClaimData.BillingProviderAddressStreet,
                BillingProviderAddressCity = payerSwitchData.BillingProviderAddressCity ?? expectedClaimData.BillingProviderAddressCity,
                BillingProviderAddressState = payerSwitchData.BillingProviderAddressState ?? expectedClaimData.BillingProviderAddressState,
                BillingProviderAddressZipCode = payerSwitchData.BillingProviderAddressZipCode ?? expectedClaimData.BillingProviderAddressZipCode,
                ServicingProviderName = payerSwitchData.ServicingProviderName ?? expectedClaimData.ServicingProviderName,
                ServicingProviderNPI = payerSwitchData.ServicingProviderNPI ?? expectedClaimData.ServicingProviderNpi,
                ServicingProviderAddressStreet = payerSwitchData.ServicingProviderAddressStreet ?? expectedClaimData.ServicingProviderAddressStreet,
                ServicingProviderAddressCity = payerSwitchData.ServicingProviderAddressCity ?? expectedClaimData.ServicingProviderAddressCity,
                ServicingProviderAddressState = payerSwitchData.ServicingProviderAddressState ?? expectedClaimData.ServicingProviderAddressState,
                ServicingProviderAddressZipCode = payerSwitchData.ServicingProviderAddressZipCode ?? expectedClaimData.ServicingProviderAddressZipCode
            };
            return payerSwitchMap;
        }
    }
}
