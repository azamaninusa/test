using FluentValidation;
using NightlyBillingData.DbModels;
using NightlyBillingValidators.FilingRuleValidators;

namespace NightlyBillingValidators
{
    public static class ValidatorFactory
    {
        private static readonly Dictionary<string, Func<IValidator<ClaimDataContainer>>> _validators = new()
        {
            { "AetnaFLPeds", () => new FilingRuleValidator_AetnaFLPeds() },
            { "BCBSKSComponent", () => new ServiceLineValidator() },
            { "BCBS SC Component", () => new ServiceLineValidator() },
            { "BCBS TX HMO Switch Payer Entity", () => new FilingRuleValidator_BCBSTXHMOSwitchPayerEntity() },
            { "BillingProviderInformationOverride", () => new FilingRuleValidator_BillingProviderInformationOverride() },
            { "ExcludeAdminCodesForVfcClaims", () => new ServiceLineValidator() },
            { "ExcludeAdultAdminCodes", () => new ServiceLineValidator() },
            { "ExcludePediatricAdminCodes", () => new ServiceLineValidator() },
            { "FacilityBasedRenderingProvider", () => new FilingRuleValidator_FacilityBasedRenderingProvider() },
            { "IncludeNDC", () => new FilingRuleValidator_IncludeNDC() },
            { "IncludeRxInProductCpts", () => new FilingRuleValidator_IncludeRxInProductCpts() },
            { "IpaBilling", () => new FilingRuleValidator_IpaBilling() },
            { "Kaiser Prevent G Codes", () => new ServiceLineValidator() },
            { "LARCNexplanonIUDInsertionCode", () => new ServiceLineValidator() },
            { "MedicareCodingOver65", () => new ServiceLineValidator() },
            { "Memorial Health Switch Payer Entity", () => new FilingRuleValidator_MemorialHealthSwitchPayerEntity() },
            { "NoAddOnCPT", () => new ServiceLineValidator() },
            { "OverrideAdminCPTs", () => new ServiceLineValidator() },
            { "RemoveServiceFacilityNpi", () => new FilingRuleValidator_RemoveServiceFacilityNpi() },
            { "Rendering NPI Switch", () => new FilingRuleValidator_RenderingNPISwitch() },
            { "RenderingProviderInformationOverride", () => new FilingRuleValidator_RenderingProviderInformationOverride() },
            { "RequireGroupID", () => new FilingRuleValidator_RequireGroupID() },
            { "RequiresZ23AndZ00129DiagCodes", () => new FilingRuleValidator_RequiresZ23AndZ00129DiagCodes() },
            { "ServiceLocationOverride", () => new FilingRuleValidator_ServiceLocationOverride() },
            { "SwitchVfcPediatricAdminCodesForAdultCodes", () => new ServiceLineValidator() },
            { "UseAdultAdminCodesForAllVaccines", () => new ServiceLineValidator() },
            { "UseGcodeAdminCPT", () => new ServiceLineValidator() }
        };

        public static IValidator<ClaimDataContainer> GetValidator(string validatorName)
        {
            if (_validators.TryGetValue(validatorName, out var validator))
                return validator();
            else
                throw new ArgumentException($"Validator {validatorName} not found");
        }
    }
}
