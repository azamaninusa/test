namespace NightlyBillingData.DbModels
{
    public class ClaimPayerFilingRules
    {
        public int PayerEntityId { get; set; }
        public int PayerFilingRuleId { get; set; }
        public string? RuleName { get; set; }
        public string? RuleDescription { get; set; }
    }
}
