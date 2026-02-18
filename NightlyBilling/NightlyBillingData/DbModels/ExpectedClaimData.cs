using System.Text.RegularExpressions;

namespace NightlyBillingData.DbModels
{
    public class ExpectedClaimData
    {
        public int ConsentFormId { get; set; }
        public string? PatientFirstName { get; set; }
        public string? PatientLastName { get; set; }
        public DateTime PatientDoB {  get; set; }
        public string? PatientGender { get; set; }
        public string? PatientAddressStreet { get; set; }
        public string? PatientAddressCity { get; set; }
        public string? PatientAddressState { get; set; }
        public string? PatientAddressZipCode { get; set; }
        public string? PrimaryInsuranceName { get; set; }
        public string? MemberId { get; set; }
        public string? GroupId { get; set; }
        public string? RelationshipToInsured { get; set; }
        public int PayerEntityId { get; set; }
        public string? BillingProviderName { get; set; }
        public string? BillingProviderNpi { get; set; }
        public string? BillingProviderEin { get; set; }
        public int BillingProviderEntityTypeCode { get; set; }
        public string? BillingProviderTaxonomyCode { get; set; }
        public string? BillingProviderAddressStreet { get; set; }
        public string? BillingProviderAddressCity { get; set; }
        public string? BillingProviderAddressState { get; set; }
        public string? BillingProviderAddressZipCode { get; set; }
        public string? PlaceOfServiceCode { get; set; }
        public string? ClaimFilingIndicatorCode { get; set; }
        public string? RenderingProviderFirstName { get; set; }
        public string? RenderingProviderLastName { get; set; }
        public string? RenderingProviderNpi { get; set; }
        public int RenderingProviderEntityTypeCode { get; set; }
        public string? RenderingProviderTaxonomyCode { get; set; }
        public string? ServicingProviderName { get; set; }
        public string? ServicingProviderNpi { get; set; }
        public int ServicingProviderEntityTypeCode { get; set; }
        public string? ServicingProviderAddressStreet { get; set; }
        public string? ServicingProviderAddressCity { get; set; }
        public string? ServicingProviderAddressState { get; set; }
        public string? ServicingProviderAddressZipCode { get; set; }

        public ExpectedClaimData() { }

        public ExpectedClaimData(ExpectedClaimData expectedClaimData)
        {
            ConsentFormId = expectedClaimData.ConsentFormId;
            PatientFirstName = expectedClaimData.PatientFirstName;
            PatientLastName = expectedClaimData.PatientLastName;
            PatientDoB = expectedClaimData.PatientDoB;
            PatientGender = expectedClaimData.PatientGender;
            PatientAddressStreet = expectedClaimData.PatientAddressStreet;
            PatientAddressCity = expectedClaimData.PatientAddressCity;
            PatientAddressState = expectedClaimData.PatientAddressState;
            PatientAddressZipCode = expectedClaimData.PatientAddressZipCode;
            PrimaryInsuranceName = expectedClaimData.PrimaryInsuranceName;
            MemberId = SanitizeField(expectedClaimData.MemberId);
            GroupId = SanitizeField(expectedClaimData.GroupId);
            RelationshipToInsured = expectedClaimData.RelationshipToInsured;
            PayerEntityId = expectedClaimData.PayerEntityId;
            BillingProviderName = expectedClaimData.BillingProviderName;
            BillingProviderNpi = expectedClaimData.BillingProviderNpi;
            BillingProviderEin = expectedClaimData.BillingProviderEin;
            BillingProviderEntityTypeCode = expectedClaimData.BillingProviderEntityTypeCode;
            BillingProviderTaxonomyCode = expectedClaimData.BillingProviderTaxonomyCode;
            BillingProviderAddressStreet = expectedClaimData.BillingProviderAddressStreet;
            BillingProviderAddressCity = expectedClaimData.BillingProviderAddressCity;
            BillingProviderAddressState = expectedClaimData.BillingProviderAddressState;
            BillingProviderAddressZipCode = expectedClaimData.BillingProviderAddressZipCode;
            PlaceOfServiceCode = expectedClaimData.PlaceOfServiceCode;
            ClaimFilingIndicatorCode = expectedClaimData.ClaimFilingIndicatorCode;
            RenderingProviderFirstName = expectedClaimData.RenderingProviderFirstName;
            RenderingProviderLastName = expectedClaimData.RenderingProviderLastName;
            RenderingProviderNpi = expectedClaimData.RenderingProviderNpi;
            RenderingProviderEntityTypeCode = expectedClaimData.RenderingProviderEntityTypeCode;
            RenderingProviderTaxonomyCode = expectedClaimData.RenderingProviderTaxonomyCode;
            ServicingProviderName = expectedClaimData.ServicingProviderName;
            ServicingProviderNpi = expectedClaimData.ServicingProviderNpi;
            ServicingProviderEntityTypeCode = expectedClaimData.ServicingProviderEntityTypeCode;
            ServicingProviderAddressStreet = expectedClaimData.ServicingProviderAddressStreet;
            ServicingProviderAddressCity = expectedClaimData.ServicingProviderAddressCity;
            ServicingProviderAddressState = expectedClaimData.ServicingProviderAddressState;
            ServicingProviderAddressZipCode = expectedClaimData.ServicingProviderAddressZipCode;
        }

        public static string SanitizeField(string fieldName)
        {
            string regex = @"[^a-zA-Z0-9]";
            return Regex.Replace(fieldName, regex, "");
        }
    }
}
