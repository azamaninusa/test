using FluentValidation;
using NightlyBillingData.DbModels;
using NightlyBillingData.ClaimFormExtensions;

namespace NightlyBillingValidators.FilingRuleValidators
{
    public class FilingRuleValidator_RequiresZ23AndZ00129DiagCodes : AbstractValidator<ClaimDataContainer>
    {
        public FilingRuleValidator_RequiresZ23AndZ00129DiagCodes()
        {
            When(x =>
                x.ActualClaimDataDto.GetPatientAgeInMonths() <= 216, () =>
                {
                    RuleForEach(x => x.ServiceDataDto)
                    .Must(x => "Z00129" == x.ActualPrimaryDiagnosisCode)
                    .WithMessage("Z10029 primary diagnosis code is missing or invalid. Expected value: {ComparisonValue}; Actual value: {PropertyValue}.");
                }).Otherwise(() =>
                {
                    RuleForEach(x => x.ServiceDataDto)
                    .Must(x => x.ExpectedPrimaryDiagnosisCode == x.ActualPrimaryDiagnosisCode)
                    .WithMessage("Z23 primary diagnosis code is missing or invalid. Expected value: Z10029; Actual value: {PropertyValue}.");
                });
        }
    }
}
