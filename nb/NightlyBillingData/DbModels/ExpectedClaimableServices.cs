namespace NightlyBillingData.DbModels
{
    public class ExpectedClaimableServices
    {
        public int VaccinationId { get; set; }
        public string? CptCodeName { get; set; }
        public string? ChargedAmount { get; set; }
        public string? UnitSize { get; set; }
        public string? Ndc { get; set; }
        public int Quantity { get; set; }
        public int ServiceType { get; set; }
        public string? DiagCode { get; set; }
        public string? UnitMeasure { get; set; }
        public string? Antigen { get; set; }
        public bool? IsPrimaryDiagCode { get; set; }
    }
}
