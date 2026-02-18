using FluentValidation;
using NightlyBillingData.DbModels;

namespace NightlyBillingValidators.FilingRuleValidators
{
    public class FilingRuleValidator_RenderingNPISwitch : AbstractValidator<ClaimDataContainer>
    {
        public FilingRuleValidator_RenderingNPISwitch()
        {
            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.FirstName ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.RenderingNPISwitchFirstName ?? string.Empty)
                .WithMessage("Rendering Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.LastNameOrOrganizationName ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.RenderingNPISwitchLastName ?? string.Empty)
                .WithMessage("Rendering Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.Npi ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.RenderingNPISwitchNPI ?? string.Empty)
                .WithMessage("Rendering Provider NPI does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.EntityType).Equal(1)
                .WithMessage("Rendering Provider Entity Type Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.TaxonomyCode.IdentificationCode ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.RenderingProviderTaxonomyCode ?? string.Empty)
                .WithMessage("Rendering Provider Taxonomy Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");
        }
    }
}
