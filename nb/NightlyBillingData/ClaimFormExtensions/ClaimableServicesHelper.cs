using VaxCare.Edi.Contracts.Claims;

namespace NightlyBillingData.ClaimFormExtensions
{
    public static class ClaimableServicesHelper
    {
        public static ClaimableServices? GetClaimableServicesHelper(this ClaimForm? claimForm, string? cptCodeName)
        {
            var services = claimForm?.Services;
            var singleService = services?.Where(x => x.CptCodeName == cptCodeName).FirstOrDefault();
            return singleService;
        }
    }
}
