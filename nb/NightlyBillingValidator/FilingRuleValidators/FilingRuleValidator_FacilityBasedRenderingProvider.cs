using FluentValidation;
using NightlyBillingData.DbModels;

namespace NightlyBillingValidators.FilingRuleValidators
{
    public class FilingRuleValidator_FacilityBasedRenderingProvider : AbstractValidator<ClaimDataContainer>
    {
        public FilingRuleValidator_FacilityBasedRenderingProvider()
        {
            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.FirstName ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.FacilityRenderingProviderFirstName ?? string.Empty)
                .WithMessage("Rendering Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.LastNameOrOrganizationName ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.FacilityRenderingProviderLastName ?? string.Empty)
                .WithMessage("Rendering Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.Npi ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.FacilityRenderingProviderNPI ?? string.Empty)
                .WithMessage("Rendering Provider NPI does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.EntityType).Equal(1)
                .WithMessage("Rendering Provider Entity Type Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.TaxonomyCode.IdentificationCode ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.RenderingProviderTaxonomyCode ?? string.Empty)
                .WithMessage("Rendering Provider Taxonomy Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");
        }
    }
}
