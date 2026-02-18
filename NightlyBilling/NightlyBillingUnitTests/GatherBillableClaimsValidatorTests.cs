using FluentValidation.TestHelper;
using NightlyBillingData.DbModels;
using NightlyBillingValidators;

namespace NightlyBillingUnitTests
{
    public class GatherBillableClaimsValidatorTests
    {
        private GatherBillableClaimsValidator Validator;
        public ClaimData claimData;

        [SetUp]
        public void Setup()
        {
            Validator = new GatherBillableClaimsValidator();
            claimData = new ClaimData();
        }

        [Test]
        public void ClaimIsNotVoided()
        {
            claimData.ConsentFormExceptionId = 0;

            var result = Validator.TestValidate(claimData);
            result.ShouldNotHaveValidationErrorFor(x => x.ConsentFormExceptionId);
        }

        [Test]
        public void ClaimIsVoided()
        {
            claimData.ConsentFormExceptionId = 1;

            var result = Validator.TestValidate(claimData);
            result.ShouldHaveValidationErrorFor(x => x.ConsentFormExceptionId);
        }

        [Test]
        public void ClaimPaymentModeInsurancePay()
        {
            claimData.ClaimPaymentMode = 0;

            var result = Validator.TestValidate(claimData);
            result.ShouldNotHaveValidationErrorFor(x => x.ClaimPaymentMode);
        }

        [Test]
        public void ClaimPaymentModeNotInsurancePay()
        {
            claimData.ClaimPaymentMode = 1;

            var result = Validator.TestValidate(claimData);
            result.ShouldHaveValidationErrorFor(x => x.ClaimPaymentMode);
        }

        [Test]
        public void ClaimCompensationStatusNotPending()
        {
            claimData.CompensationStatus = "Processing";

            var result = Validator.TestValidate(claimData);
            result.ShouldNotHaveValidationErrorFor(x => x.CompensationStatus);
        }

        [Test]
        public void ClaimCompensationStatusIsPending()
        {
            claimData.CompensationStatus = "Pending";

            var result = Validator.TestValidate(claimData);
            result.ShouldHaveValidationErrorFor(x => x.CompensationStatus);
        }

        [Test]
        public void ClaimPartnerIsNotTestPartner()
        {
            claimData.IsTestPartner = false;

            var result = Validator.TestValidate(claimData);
            result.ShouldNotHaveValidationErrorFor(x => x.IsTestPartner);
        }

        [Test]
        public void ClaimPartnerIsTestPartner()
        {
            claimData.IsTestPartner = true;

            var result = Validator.TestValidate(claimData);
            result.ShouldHaveValidationErrorFor(x => x.IsTestPartner);
        }

        [Test]
        public void ClaimActionNotEqualHoldClaim()
        {
            claimData.ClaimActionId = 1; //Correction

            var result = Validator.TestValidate(claimData);
            result.ShouldNotHaveValidationErrorFor(x => x.ClaimActionId);
        }

        [Test]
        public void ClaimActionEqualsHoldClaim()
        {
            claimData.ClaimActionId = 4; //Hold Claim

            var result = Validator.TestValidate(claimData);
            result.ShouldHaveValidationErrorFor(x => x.ClaimActionId);
        }

        [Test]
        public void ClaimHasCriticalExceptions()
        {
            claimData.ValidationLevel = 3;

            var result = Validator.TestValidate(claimData);
            result.ShouldHaveValidationErrorFor(x => x.ValidationLevel);
        }

        [Test]
        public void ClaimHasNoCriticalExceptions()
        {
            claimData.ValidationLevel = 0;

            var result = Validator.TestValidate(claimData);
            result.ShouldNotHaveValidationErrorFor(x => x.ValidationLevel);
        }

        [Test]
        public void ClaimIsNotInReview()
        {
            claimData.IsInReview = false;

            var result = Validator.TestValidate(claimData);
            result.ShouldNotHaveValidationErrorFor(x => x.IsInReview);
        }

        [Test]
        public void ClaimIsInReview()
        {
            claimData.IsInReview = true;

            var result = Validator.TestValidate(claimData);
            result.ShouldHaveValidationErrorFor(x => x.IsInReview);
        }

        [Test]
        public void ClaimContainsAtLeastOneProduct()
        {
            claimData.ProductCount = 3;

            var result = Validator.TestValidate(claimData);
            result.ShouldNotHaveValidationErrorFor(x=> x.ProductCount);
        }

        [Test]
        public void ClaimContainsNoProducts()
        {
            claimData.ProductCount = 0;

            var result = Validator.TestValidate(claimData);
            result.ShouldHaveValidationErrorFor(x => x.ProductCount);
        }

        [Test]
        public void CleanClaimNoExceptions()
        {
            claimData.ValidationLevel = 0;

            var result = Validator.TestValidate(claimData);
            result.ShouldNotHaveValidationErrorFor(x => x.ValidationLevel);
        }

        [Test]
        public void ClaimWithForceableException()
        {
            claimData.ValidationLevel = 1;
            claimData.ClaimActionId = 3;

            var result = Validator.TestValidate(claimData);
            result.ShouldNotHaveValidationErrorFor(GatherBillableClaimsValidator.HasForceableClaimAction(claimData).ToString());
        }

        [Test]
        public void ClaimWithSuperforceableException()
        {
            claimData.ValidationLevel = 2;
            claimData.ClaimActionId = 6;

            var result = Validator.TestValidate(claimData);
            result.ShouldNotHaveValidationErrorFor(GatherBillableClaimsValidator.HasSuperforceableClaimAction(claimData).ToString());
        }
    }
}
