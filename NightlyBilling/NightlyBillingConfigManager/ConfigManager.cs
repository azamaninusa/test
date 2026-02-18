using System.Configuration;

namespace NightlyBillingConfigManager
{
    public static class ConfigManager
    {
        public static string GetConnectionString(string environment)
        {
            string connectionString = string.Empty;
            if (environment == "qa")
                connectionString = ConfigurationManager.ConnectionStrings["QA"].ConnectionString;
            else if (environment == "stg" || environment == "staging")
                connectionString = ConfigurationManager.ConnectionStrings["STG"].ConnectionString;
            else
            {
                Console.WriteLine("Value for environment is not valid. Defaulting to QA environment.");
                connectionString = ConfigurationManager.ConnectionStrings["QA"].ConnectionString;
            }
            return connectionString;
        }
    }
}
