namespace NightlyBillingData.DbModels
{
    public class ClaimTestScenarios
    {
        public string? AgeGroup { get; set; }
        public string? AdminScenario { get; set; }
        public int ConsentFormId { get; set; }
        public int PayerEntityId { get; set; }
        public DateTime DoS { get; set; }
        public DateTime DoB { get; set; }
        public int AgeDoS { get; set; }
        public int AgeMonths { get; set; }
        public string? RelationshipToInsured { get; set; }
        public int DoseCount { get; set; }
        public string? VaccineCodeName { get; set; }
        public int ProductId { get; set; }
        public string? Antigen { get; set; }
        public int Components { get; set; }
        public int TotalComponents { get; set; }
        public string? Stock { get; set; }
        public string? VisitType { get; set; }
        public bool PediatricSingleComponentBillingFF { get; set; }
        public string? FilingRuleName { get; set; }
    }
}
