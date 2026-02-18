using Dapper;
using NightlyBillingData.DbModels;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;

namespace NightlyBillingData
{
    public class SqlQueries
    {
        private string? ConnectionString { get; set; }

        public SqlQueries(string connectionString)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public SqlQueries()
        {

        }

        public ResiliencePipeline GetPipeline()
        {
            // Performs retries if queries return null. 
            var pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = args => args.Outcome switch
                    {
                        { Exception: SqlException } => PredicateResult.True(),
                        { Exception: Exception } => PredicateResult.True(),
                        _ => PredicateResult.False()
                    },
                    BackoffType = DelayBackoffType.Constant,
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(3)
                })
                .Build();
            return pipeline;
        }

        #region Generics
        //  Determines if query output is a list
        public bool IsList<T>(T o)
        {
            var type = o.GetType();
            return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)));
        }

        //  Generics to generate a list of values or a single output based on inputs
        private List<T> GetListData<T>(string query)
        {
            return GetListData<T, int?>(query, null);
        }

        private List<T> GetListData<T, K>(string query, K? id)
        {
            return QueryData<T, K?>(query, id);
        }

        private T? GetDataById<T, K>(string query, K? id)
        {
            return QueryData<T, K>(query, id).FirstOrDefault();
        }

        private List<T> QueryData<T, K>(string query, K? id)
        {
            var data = new List<T>();

            var pipeline = GetPipeline();
            pipeline.Execute(token =>
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    data = connection.Query<T>(query, id is not null ? new {id} : null).ToList();                                   
                }
                if (data == null)
                {
                    Console.WriteLine($"Unable to get {typeof(T)}\n");
                    throw new Exception("Query results are null. Retrying");
                }
            });
            return data;
        }
        #endregion

        //  Query to execute primary GatherBillableClaims sproc
        public List<BillableClaims> GetBillableClaims()
        {
            return GetListData<BillableClaims>(SqlData.getBillableClaims);
        }

        public List<ClaimData> GetClaimData(List<BillableClaims> billableClaims)
        {
            var claimData = new List<ClaimData>();
            foreach (var claim in billableClaims)
            {
                var billableClaimData = GetDataById<ClaimData, int>(SqlData.getClaimData, claim.ConsentFormId);
                if (billableClaimData is not null)
                {
                    billableClaimData.ClaimExceptions = GetListData<ClaimExceptions, int>(SqlData.getClaimExceptions, claim.ConsentFormId);
                    claimData.Add(billableClaimData);
                }
            }
            return claimData;
        }

        public List<ClaimExceptions> GetClaimExceptions(List<BillableClaims> billableClaims)
        {
            var claimExceptions = new List<ClaimExceptions>();
            foreach (var claim in billableClaims)
            {
                var exceptions = GetListData<ClaimExceptions, int>(SqlData.getClaimExceptions, claim.ConsentFormId);
                claimExceptions.AddRange(exceptions);
            }
            return claimExceptions;
        }

        public List<ClaimTestScenarios>? GetClaimTestScenarios(string filingRuleName)
        {
            return GetListData<ClaimTestScenarios, string>(SqlData.getTestScenarioData, filingRuleName);
        }

        public List<MessageOut>? GetMessageOut(List<ClaimTestScenarios> claimTestScenarios)
        {
            var claim = claimTestScenarios.Select(x => x.ConsentFormId).FirstOrDefault();
            var messageOut = GetListData<MessageOut, int>(SqlData.getMessageOut, claim);
            return messageOut;
        }

        public List<ExpectedClaimData>? GetExpectedClaimData(List<ClaimTestScenarios> claimTestScenarios)
        {
            var consents = string.Join(",", claimTestScenarios.Select(x => x.ConsentFormId).ToList());
            var claims = GetListData<ExpectedClaimData, string>(SqlData.getExpectedClaimData, consents);
            return claims;
        }

        public List<ProviderOverrideData>? GetProviderOverrideData(int claimId)
        {
            var claims = GetListData<ProviderOverrideData, int>(SqlData.getProviderOverrides, claimId);
            return claims;
        }

        public List<PayerSwitchData>? GetPayerSwitchData(int claimId)
        {
            var claims = GetListData<PayerSwitchData, int>(SqlData.getPayerSwitchData, claimId);
            return claims;
        }

        public List<ExpectedClaimableServices>? GetClaimServices(int claimId)
        {
            var services = GetListData<ExpectedClaimableServices, int>(SqlData.getClaimServicesData, claimId);
            return services;
        }
    }
}
