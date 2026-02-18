namespace NightlyBillingData.DbModels
{
    public class ServicesDataContainer
    {
        public class Services
        {
            public List<ServiceData?> ServiceData { get; set; } = new List<ServiceData?>();

            public Services() { }
        }

        public class ServiceData
        {
            public string? CptCode { get; set; }
            public string? ExpectedNdc { get; set; }
            public string? ActualNdc { get; set; }
            public string? ExpectedRxNumber { get; set; }
            public string? ActualRxNumber { get; set; }
            public string? ExpectedPrimaryDiagnosisCode { get; set; }
            public string? ActualPrimaryDiagnosisCode { get; set; }

            public ServiceData() { }
        }
    }
}
