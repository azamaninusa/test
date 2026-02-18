using VaxCare.Edi.Contracts.Claims;

namespace NightlyBillingData.ClaimFormExtensions
{
    public static class GetPatientAgeInMonthsHelper
    {
        public static int GetPatientAgeInMonths(this ClaimForm claimForm)
        {
            var claimDoS = claimForm.DateOfService;
            var patientDoB = claimForm.Patient.DoB.GetValueOrDefault();
            int ageInMonths = (claimDoS.Year - patientDoB.Year) * 12;

            if (claimDoS < patientDoB)
            {
                ageInMonths--;
            }
            return ageInMonths;
        }
    }
}
