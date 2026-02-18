using CommandLine;
using NightlyBillingTestRunner;
using NightlyBillingConfigManager;
using NightlyBillingData;
using NightlyBillingValidators;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Setup
        var opts = Parser.Default.ParseArguments<RunOptions>(args);
        string env = opts.Value.Environment.ToLower();
        string connectionString = ConfigManager.GetConnectionString(env);
        var queries = new SqlQueries(connectionString);
        Console.WriteLine("Environment where tests will be executed: " + env);

        // Execute GatherBillableClaims
        Console.WriteLine("Executing GatherBillableClaims.");
        var billableClaims = queries.GetBillableClaims();
        var claimCount = billableClaims.Count();
        Console.WriteLine($"Query returned {claimCount} claims.");

        //  Run queries to gather claim data
        Console.WriteLine("Executing query to gather claim data.");
        var claimData = queries.GetClaimData(billableClaims);

        //  Run validation on each claim
        Console.WriteLine("Running validation tests.");
        foreach (var claim in claimData)
        {
            var validator = new GatherBillableClaimsValidator();
            var results = validator.Validate(claim);

            if (!results.IsValid)
            {
                foreach (var failure in results.Errors)
                {
                    Console.WriteLine("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
                }
            }
            else
            {
                Console.WriteLine("Validation successful.");
            }
        }
    }
}
