using FluentValidation;
using NightlyBillingData.ClaimFormExtensions;
using NightlyBillingData.DbModels;

namespace NightlyBillingValidators.FilingRuleValidators
{
    public class ServiceLineValidator : AbstractValidator<ClaimDataContainer>
    {
        public ServiceLineValidator()
        {
            RuleFor(x => x.ActualClaimDataDto.GetCptCodeHelper(x.ExpectedCptCode)).NotNull().Equal(x => x.ExpectedCptCode)
                .WithMessage("Admin CPT Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.GetServiceLineQuantityHelper(x.ExpectedCptCode)).NotNull().Equal(x => x.ExpectedQuantity)
                .WithMessage("Admin CPT Quantity does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.GetServiceLineChargeHelper(x.ExpectedCptCode)).NotNull().Equal(x => x.ExpectedServiceLineCharge)
                .WithMessage("Admin CPT Billed Amount does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");
        }
    }
}
