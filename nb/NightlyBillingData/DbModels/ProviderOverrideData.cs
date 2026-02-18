namespace NightlyBillingData.DbModels
{
    public class ProviderOverrideData
    {
        public int ConsentFormId { get; set; }
        public int PartnerId { get; set; }
        public int ClinicId { get; set; }
        public int PayerEntityId { get; set; }
        public int ProviderId { get; set; }
        public int EicId { get; set; }
        public int InformationEntityId { get; set; }
        public int EntityAddressId { get; set; }
        public string? RenderingNPISwitchNPI { get; set; }
        public string? RenderingNPISwitchFirstName { get; set; }
        public string? RenderingNPISwitchLastName { get; set; }
        public string? FacilityRenderingProviderNPI { get; set; }
        public string? FacilityRenderingProviderFirstName { get; set; }
        public string? FacilityRenderingProviderLastName { get; set; }
        public string? BillingProviderOverrideNPI { get; set; }
        public string? BillingProviderOverrideEIN { get; set; }
        public string? BillingProviderOverrideLastName { get; set; }
        public string? BillingProviderOverrideAddressStreet { get; set; }
        public string? BillingProviderOverrideCity { get; set; }
        public string? BillingProviderOverrideState { get; set; }
        public string? BillingProviderOverrideZipCode { get; set; }
        public string? RenderingProviderOverrideNPI { get; set; }
        public string? RenderingProviderOverrideFirstName { get; set; }
        public string? RenderingProviderOverrideLastName { get; set; }
        public string? ServicingProviderOverrideNPI { get; set; }
        public string? ServicingProviderOverrideLastName { get; set; }
        public string? ServicingProviderOverrideAddressStreet { get; set; }
        public string? ServicingProviderOverrideCity { get; set; }
        public string? ServicingProviderOverrideState { get; set; }
        public string? ServicingProviderOverrideZipCode { get; set; }
        public string? BillingProviderTaxonomyCode { get; set; }
        public string? RenderingProviderTaxonomyCode { get; set; }
    }
}
