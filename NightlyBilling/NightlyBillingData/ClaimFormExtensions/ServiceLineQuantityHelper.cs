using VaxCare.Edi.Contracts.Claims;

namespace NightlyBillingData.ClaimFormExtensions
{
    public static class ServiceLineQuantityHelper
    {
        public static string GetServiceLineQuantityHelper(this ClaimForm? claimForm, string? cptCodeName)
        {
            var serviceLine = claimForm.GetClaimableServicesHelper(cptCodeName);
            if (serviceLine == null)
            {
                return string.Empty;
            }
            return serviceLine.Quantity.ToString();
        }
    }
}
