using FluentValidation;
using NightlyBillingData.DbModels;

namespace NightlyBillingValidators.FilingRuleValidators
{
    public class FilingRuleValidator_ServiceLocationOverride : AbstractValidator<ClaimDataContainer>
    {
        public FilingRuleValidator_ServiceLocationOverride()
        {
            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.LastNameOrOrganizationName).NotNull().Equal(x => x.ProviderOverrideDataDto.ServicingProviderOverrideLastName)
                .WithMessage("Servicing Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.Npi ?? string.Empty).Equal(x => x.ProviderOverrideDataDto.ServicingProviderOverrideNPI ?? string.Empty)
                .WithMessage("Servicing Provider Npi does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.EntityType).NotNull().Equal(x => x.ExpectedClaimDataDto.ServicingProviderEntityTypeCode)
                .WithMessage("Servicing Provider Entity Type Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.Address1).NotNull().Equal(x => x.ProviderOverrideDataDto.ServicingProviderOverrideAddressStreet)
                .WithMessage("Servicing Provider Address (Street) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.City).NotNull().Equal(x => x.ProviderOverrideDataDto.ServicingProviderOverrideCity)
                .WithMessage("Servicing Provider Address (City) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.State).NotNull().Equal(x => x.ProviderOverrideDataDto.ServicingProviderOverrideState)
                .WithMessage("Servicing Provider Address (State) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.ZipCode.Substring(0, 5)).NotNull().Equal(x => x.ProviderOverrideDataDto.ServicingProviderOverrideZipCode.Substring(0, 5))
                .WithMessage("Servicing Provider Address (ZipCode) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");
        }
    }
}
