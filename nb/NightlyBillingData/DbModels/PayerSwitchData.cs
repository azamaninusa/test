namespace NightlyBillingData.DbModels
{
    public class PayerSwitchData
    {
        public int ConsentFormId { get; set; }
        public int PartnerId { get; set; }
        public int ClinicId { get; set; }
        public int PayerSwitchMapFromPayerEntityId { get; set; }
        public int PayerSwitchMapToPayerEntityId { get; set; }
        public string? PayerSwitchMapToPayerName { get; set; }
        public string? BillingProviderName { get; set; }
        public string? BillingProviderNPI { get; set; }
        public string? BillingProviderEIN { get; set; }
        public string? BillingProviderTaxonomyCode { get; set; }
        public string? BillingProviderAddressStreet { get; set; }
        public string? BillingProviderAddressCity { get; set; }
        public string? BillingProviderAddressState { get; set; }
        public string? BillingProviderAddressZipCode { get; set; }
        public string? ServicingProviderName { get; set; }
        public string? ServicingProviderNPI { get; set; }
        public string? ServicingProviderAddressStreet { get; set; }
        public string? ServicingProviderAddressCity { get; set; }
        public string? ServicingProviderAddressState { get; set; }
        public string? ServicingProviderAddressZipCode { get; set; }
    }
}
