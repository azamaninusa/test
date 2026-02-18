using FluentValidation;
using NightlyBillingData.DbModels;
using NightlyBillingData.ClaimFormExtensions;

namespace NightlyBillingValidators.FilingRuleValidators
{
    public class FilingRuleValidator_AetnaFLPeds : AbstractValidator<ClaimDataContainer>
    {
        public FilingRuleValidator_AetnaFLPeds()
        {
            When(x =>
                x.ActualClaimDataDto.GetPatientAgeInMonths() <= 216, () =>
                {
                    RuleFor(x => x.ActualClaimDataDto.PayerName.ToUpper()).NotNull().Equal("AETNA (PEDS FL)")
                    .WithMessage("Primary Insurance Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");
                }).Otherwise(() =>
                {
                    RuleFor(x => x.ActualClaimDataDto.PayerName.ToUpper()).NotNull().Equal(x => x.ExpectedClaimDataDto.PrimaryInsuranceName.ToUpper())
                    .WithMessage("Primary Insurance Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");
                });
        }
    }
}
