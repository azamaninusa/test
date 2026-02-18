using FluentValidation;
using NightlyBillingData.DbModels;

namespace NightlyBillingValidators.FilingRuleValidators
{
    public class FilingRuleValidator_RenderingProviderInformationOverride : AbstractValidator<ClaimDataContainer>
    {
        public FilingRuleValidator_RenderingProviderInformationOverride()
        {
            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.FirstName ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.RenderingProviderOverrideFirstName ?? string.Empty)
                .WithMessage("Rendering Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.LastNameOrOrganizationName ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.RenderingProviderOverrideLastName ?? string.Empty)
                .WithMessage("Rendering Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.Npi ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.RenderingProviderOverrideNPI ?? string.Empty)
                .WithMessage("Rendering Provider NPI does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.EntityType).Equal(x => x.ExpectedClaimDataDto.RenderingProviderEntityTypeCode)
                .WithMessage("Rendering Provider Entity Type Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.TaxonomyCode.IdentificationCode ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.RenderingProviderTaxonomyCode ?? string.Empty)
                .WithMessage("Rendering Provider Taxonomy Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");
        }
    }
}
