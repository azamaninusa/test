using FluentValidation;
using NightlyBillingData.DbModels;

namespace NightlyBillingValidators.FilingRuleValidators
{
    public class FilingRuleValidator_RemoveServiceFacilityNpi : AbstractValidator<ClaimDataContainer>
    {
        public FilingRuleValidator_RemoveServiceFacilityNpi()
        {
            RuleFor(x => x.ActualClaimDataDto.PlaceOfService).NotNull().Equal(x => x.ExpectedClaimDataDto.PlaceOfServiceCode)
                .WithMessage("Place Of Service Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.LastNameOrOrganizationName).NotNull().Equal(x => x.ExpectedClaimDataDto.ServicingProviderName)
                .WithMessage("Servicing Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.Npi ?? string.Empty).Equal(x => string.Empty)
                .WithMessage("Servicing Provider Npi does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.EntityType).NotNull().Equal(x => x.ExpectedClaimDataDto.ServicingProviderEntityTypeCode)
                .WithMessage("Servicing Provider Entity Type Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.Address1).NotNull().Equal(x => x.ExpectedClaimDataDto.ServicingProviderAddressStreet)
                .WithMessage("Servicing Provider Address (Street) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.City).NotNull().Equal(x => x.ExpectedClaimDataDto.ServicingProviderAddressCity)
                .WithMessage("Servicing Provider Address (City) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.State).NotNull().Equal(x => x.ExpectedClaimDataDto.ServicingProviderAddressState)
                .WithMessage("Servicing Provider Address (State) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.ZipCode.Substring(0, 5)).NotNull().Equal(x => x.ExpectedClaimDataDto.ServicingProviderAddressZipCode.Substring(0, 5))
                .WithMessage("Servicing Provider Address (ZipCode) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");
        }
    }
}
