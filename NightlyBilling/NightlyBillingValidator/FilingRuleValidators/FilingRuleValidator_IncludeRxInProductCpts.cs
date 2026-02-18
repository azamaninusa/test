using FluentValidation;
using NightlyBillingData.DbModels;

namespace NightlyBillingValidators.FilingRuleValidators
{
    public class FilingRuleValidator_IncludeRxInProductCpts : AbstractValidator<ClaimDataContainer>
    {
        public FilingRuleValidator_IncludeRxInProductCpts()
        {
            RuleForEach(x => x.ServiceDataDto)
            .Must(x => x.ExpectedRxNumber == x.ActualRxNumber)
            .WithMessage("RxNumber is missing or invalid. Expected value: {ComparisonValue}; Actual value: {PropertyValue}.");
        }
    }
}
