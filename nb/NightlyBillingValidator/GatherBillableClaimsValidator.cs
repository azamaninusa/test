using FluentValidation;
using NightlyBillingData.DbModels;

namespace NightlyBillingValidators
{
    public class GatherBillableClaimsValidator : AbstractValidator<ClaimData>
    {
        public GatherBillableClaimsValidator()
        {
            RuleFor(claimData => claimData.ConsentFormExceptionId).Equal(0)
                .WithMessage("Claim should not be voided.");

            RuleFor(claimData => claimData.ClaimPaymentMode).Equal(0)
                .WithMessage("Claim Payment Mode should be Insurance Pay.");

            RuleFor(claimData => claimData.CompensationStatus).NotEqual("Pending")
                .WithMessage("Claim should not have a Compensation Status of Pending.");

            RuleFor(claimData => claimData.IsTestPartner).Equal(false)
                .WithMessage("Claim should not belong to a Test Partner.");

            RuleFor(claimData => claimData.ClaimActionId).NotEqual(4)
                .WithMessage("Claim Action should not equal Hold Claim.");

            RuleFor(claimData => claimData.IsInReview).Equal(false)
                .WithMessage("Claim cannot be in an In Review state.");

            RuleFor(claimData => claimData.ProductCount).NotEqual(0)
                .WithMessage("Claim must contain at at least one product.");

            RuleFor(claimData => claimData.ValidationLevel).NotEqual(3)
                .WithMessage("Claim cannot have any 'Critical' exceptions.");

            When(claimData =>
                claimData.ClaimActionId.Equals(0), () =>
                {
                    RuleFor(claimData => claimData.ValidationLevel).Equal(0)
                    .WithMessage("If claim has no Claim Action assigned, the Validation Level of any exceptions should be zero.");
                });

            When(claimData => 
                claimData.ValidationLevel.Equals(1), () =>
                {
                    RuleFor(claimData => HasForceableClaimAction(claimData)).Equal(true)
                    .WithMessage("If a claim contains Forceable exceptions, the claim Action should be anything other than Hold Claim.");
                });

            When(claimData =>
                claimData.ValidationLevel.Equals(2), () =>
                {
                    RuleFor(claimData => HasSuperforceableClaimAction(claimData)).Equal(true)
                    .WithMessage("If a claim contains Superforceable exceptions, the claim Action should be anything other than Hold Claim.");
                });
        }

        public static bool HasSuperforceableClaimAction(ClaimData claimData)
        {
            if (claimData.ClaimActionId == 1 || claimData.ClaimActionId == 2 || claimData.ClaimActionId == 6 || claimData.ClaimActionId == 8)
                return true;
            return false;
        }

        public static bool HasForceableClaimAction(ClaimData claimData)
        {
            if (HasSuperforceableClaimAction(claimData) == true || claimData.ClaimActionId == 3 || claimData.ClaimActionId == 5)
                return true;
            return false;
        }
    }
}
