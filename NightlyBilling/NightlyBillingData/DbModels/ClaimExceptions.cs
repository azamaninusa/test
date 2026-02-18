namespace NightlyBillingData.DbModels
{
    public class ClaimExceptions
    {
        public int ConsentFormId { get; set; }
        public int QaExceptionId { get; set; }
        public int QaExceptionTypeId { get; set; }
        public string? QaExceptionTypeName { get; set; }
        public int QaExceptionCategoryId { get; set; }
        public int ValidationTypeId { get; set; }
    }
}
