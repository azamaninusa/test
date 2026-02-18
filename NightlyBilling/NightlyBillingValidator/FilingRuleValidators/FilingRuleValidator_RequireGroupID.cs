using FluentValidation;
using NightlyBillingData.DbModels;

namespace NightlyBillingValidators.FilingRuleValidators
{
    public class FilingRuleValidator_RequireGroupID : AbstractValidator<ClaimDataContainer>
    {
        public FilingRuleValidator_RequireGroupID()
        {
            RuleFor(x => x.ActualClaimDataDto.PolicyGroup).NotNull().Equal(x => x.ExpectedClaimDataDto.GroupId)
                .WithMessage("Filing Rule RequiresGroupID failed validation. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");
        }
    }
}
