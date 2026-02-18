namespace NightlyBillingData.DbModels
{
    public class ClaimData : ClaimExceptions
    {
        public int ConsentFormId { get; set; }
        public int ClaimPaymentMode { get; set; }
        public string? PaymentModeReason { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DateOfService { get; set; }
        public bool IsSent { get; set; }
        public string? CompensationStatus { get; set; }
        public string? CompensationSubStatus { get; set; }
        public string? FinalReviewDecision { get; set; }
        public string? FinalReviewDecisionReason { get; set; }
        public bool IsInReview { get; set; }
        public string? DenialAction { get; set; }
        public int ClaimActionId { get; set; }
        public int PatientVisitId { get; set; }
        public int Stock { get; set; }
        public string? ResearchStatus { get; set; }
        public string? BillingStatus { get; set; }
        public bool IsPaidOrPartial => (ResearchStatus != null && (ResearchStatus == "PAID" || ResearchStatus == "PARTIAL"))
                                        || (BillingStatus != null && (BillingStatus == "835 Paid" || BillingStatus == "835 Partial"));
        public int BatchId { get; set; }
        public string? EligibilityId { get; set; }
        public int ConsentFormExceptionId { get; set; }
        public bool IsTestPartner { get; set; }
        public int ProductCount { get; set; }
        public int ValidationLevel { get; set; }
        public List<ClaimExceptions>? ClaimExceptions { get; set; }

        public ClaimData() { }

        public ClaimData(ClaimData data, List<ClaimExceptions>? claimExceptions)
        {
            ConsentFormId = data.ConsentFormId;
            ClaimPaymentMode = data.ClaimPaymentMode;
            PaymentModeReason = data.PaymentModeReason;
            CreatedDate = data.CreatedDate;
            DateOfService = data.DateOfService;
            IsSent = data.IsSent;
            CompensationStatus = data.CompensationStatus;
            CompensationSubStatus = data.CompensationSubStatus;
            FinalReviewDecision = data.FinalReviewDecision;
            FinalReviewDecisionReason = data.FinalReviewDecisionReason;
            IsInReview = data.IsInReview;
            DenialAction = data.DenialAction;
            ClaimActionId = data.ClaimActionId;
            PatientVisitId = data.PatientVisitId;
            Stock = data.Stock;
            ResearchStatus = data.ResearchStatus;
            BillingStatus = data.BillingStatus;
            BatchId = data.BatchId;
            EligibilityId = data.EligibilityId;
            ConsentFormExceptionId = data.ConsentFormExceptionId;
            IsTestPartner = data.IsTestPartner;
            ProductCount = data.ProductCount;
            ValidationLevel = data.ValidationLevel;
            ClaimExceptions = claimExceptions;
        }
    }
}
