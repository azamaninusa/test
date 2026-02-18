using FluentValidation;
using NightlyBillingData.DbModels;

namespace NightlyBillingValidators.FilingRuleValidators
{
    public class FilingRuleValidator_IpaBilling : AbstractValidator<ClaimDataContainer>
    {
        public FilingRuleValidator_IpaBilling()
        {
            RuleFor(x => x.ActualClaimDataDto.PayerName.ToUpper()).NotNull().Equal(x => x.PayerSwitchDataDto.PayerSwitchMapToPayerName.ToUpper())
                .WithMessage("Primary Insurance Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.LastNameOrOrganizationName).NotNull().Equal(x => x.PayerSwitchDataDto.BillingProviderName)
                .WithMessage("Billing Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.Npi).NotNull().Equal(x => x.PayerSwitchDataDto.BillingProviderNPI)
                .WithMessage("Billing Provider NPI does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.FederalTaxIDNumber).NotNull().Equal(x => x.PayerSwitchDataDto.BillingProviderEIN)
                .WithMessage("Billing Provider EIN does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.TaxonomyCode.IdentificationCode ?? string.Empty).Equal(x => x.PayerSwitchDataDto.BillingProviderTaxonomyCode ?? string.Empty)
                .WithMessage("Billing Provider Taxonomy Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.Address1).NotNull().Equal(x => x.PayerSwitchDataDto.BillingProviderAddressStreet)
                .WithMessage("Billing Provider Address (Street) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.City).NotNull().Equal(x => x.PayerSwitchDataDto.BillingProviderAddressCity)
                .WithMessage("Billing Provider Address (City) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.State).NotNull().Equal(x => x.PayerSwitchDataDto.BillingProviderAddressState)
                .WithMessage("Billing Provider Address (State) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.ZipCode.Substring(0, 5)).NotNull().Equal(x => x.PayerSwitchDataDto.BillingProviderAddressZipCode.Substring(0, 5))
                .WithMessage("Billing Provider Address (ZipCode) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.LastNameOrOrganizationName).NotNull().Equal(x => x.PayerSwitchDataDto.ServicingProviderName)
                .WithMessage("Servicing Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.Npi ?? string.Empty).Equal(x => x.PayerSwitchDataDto.ServicingProviderNPI ?? string.Empty)
                .WithMessage("Servicing Provider Npi does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.Address1).NotNull().Equal(x => x.PayerSwitchDataDto.ServicingProviderAddressStreet)
                .WithMessage("Servicing Provider Address (Street) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.City).NotNull().Equal(x => x.PayerSwitchDataDto.ServicingProviderAddressCity)
                .WithMessage("Servicing Provider Address (City) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.State).NotNull().Equal(x => x.PayerSwitchDataDto.ServicingProviderAddressState)
                .WithMessage("Servicing Provider Address (State) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.ZipCode.Substring(0, 5)).NotNull().Equal(x => x.PayerSwitchDataDto.ServicingProviderAddressZipCode.Substring(0, 5))
                .WithMessage("Servicing Provider Address (ZipCode) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");
        }
    }
}
