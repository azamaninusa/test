using VaxCare.Edi.Contracts.Claims;

namespace NightlyBillingData.ClaimFormExtensions
{
    public static class ServiceLineChargeHelper
    {
        public static string GetServiceLineChargeHelper(this ClaimForm? claimForm, string? cptCodeName)
        {
            var serviceLine = claimForm.GetClaimableServicesHelper(cptCodeName);
            if (serviceLine == null)
            {
                return string.Empty;
            }
            return serviceLine.ChargedAmount.ToString() ?? string.Empty;
        }
    }
}
