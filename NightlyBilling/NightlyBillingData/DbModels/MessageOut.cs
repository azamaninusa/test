namespace NightlyBillingData.DbModels
{
    public class MessageOut
    {
        public int MsgId { get; set; }
        public int ConsentFormId { get; set; }
        public int ClaimTypeQualifierId { get; set; }
        public string? ControlNumber { get; set; }
        public int BillingProcessorId { get; set; }
        public DateTime? CreatedOnUtc { get; set; }
        public string? ISAID { get; set; }
        public string? MessageData { get; set; }
    }
}
