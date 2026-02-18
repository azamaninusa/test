using FluentValidation;
using NightlyBillingData.DbModels;

namespace NightlyBillingValidators.FilingRuleValidators
{
    public class FilingRuleValidator_IncludeNDC : AbstractValidator<ClaimDataContainer>
    {
        public FilingRuleValidator_IncludeNDC()
        {
            RuleForEach(x => x.ServiceDataDto)
            .Must(x => "N4" + x.ExpectedNdc == x.ActualNdc)
            .WithMessage("NDC code is missing or invalid. Expected value: {ComparisonValue}; Actual value: {PropertyValue}.");
        }
    }
}
