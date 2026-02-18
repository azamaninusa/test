using FluentValidation;
using NightlyBillingData.DbModels;

namespace NightlyBillingValidators.FilingRuleValidators
{
    public class FilingRuleValidator_BillingProviderInformationOverride : AbstractValidator<ClaimDataContainer>
    {
        public FilingRuleValidator_BillingProviderInformationOverride()
        {
            RuleFor(x => x.ActualClaimDataDto.BillingProvider.LastNameOrOrganizationName).NotNull().Equal(x => x.ProviderOverrideDataDto.BillingProviderOverrideLastName)
                .WithMessage("Billing Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.Npi).NotNull().Equal(x => x.ProviderOverrideDataDto.BillingProviderOverrideNPI)
                .WithMessage("Billing Provider NPI does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.FederalTaxIDNumber).NotNull().Equal(x => x.ProviderOverrideDataDto.BillingProviderOverrideEIN)
                .WithMessage("Billing Provider EIN does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.EntityType).NotNull().Equal(x => x.ExpectedClaimDataDto.BillingProviderEntityTypeCode)
                .WithMessage("Billing Provider Entity Type Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.TaxonomyCode.IdentificationCode ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.BillingProviderTaxonomyCode ?? string.Empty)
                .WithMessage("Billing Provider Taxonomy Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.Address1).NotNull().Equal(x => x.ProviderOverrideDataDto.BillingProviderOverrideAddressStreet)
                .WithMessage("Billing Provider Address (Street) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.City).NotNull().Equal(x => x.ProviderOverrideDataDto.BillingProviderOverrideCity)
                .WithMessage("Billing Provider Address (City) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.State).NotNull().Equal(x => x.ProviderOverrideDataDto.BillingProviderOverrideState)
                .WithMessage("Billing Provider Address (State) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.ZipCode.Substring(0, 5)).NotNull().Equal(x => x.ProviderOverrideDataDto.BillingProviderOverrideZipCode.Substring(0, 5))
                .WithMessage("Billing Provider Address (ZipCode) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");
        }
    }
}
