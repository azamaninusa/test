using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using VaxCare.ApiClient;
using VaxCare.ApiClient.TokenGenerator;
using VaxCare.ApiClient.TokenGenerator.Models;
using VaxCare.Core.ApiRequests.PatientsApi;
using VaxCare.Core.Logger;
using VaxCare.Core.WebDriver;
using VaxCare.Data;
using VaxCare.Data.DataEntry;
using VaxCare.Data.HealthSystems;
using VaxCare.Data.Reporting;
using VaxCare.Data.Repositories;
using VaxCare.Data.Risk;
using VaxCare.Data.Sales;

namespace VaxCare.Core
{
    public class TestStartup
    {
        public const string DriverSection = "WebDriverSettings";
        public const string OktaConfigSection = "OktaConfiguration";

        public IConfigurationRoot Configuration { get; }
        public IServiceProvider Services { get; }

        public TestStartup()
        {
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json",
                optional: false, reloadOnChange: true);
            Configuration = configBuilder.Build();

            var services = new ServiceCollection();
            services.AddSingleton(Configuration);
            services.AddHttpClient<IApi, Api>();
            services.AddSingleton<IClient, Client>();
            services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
            services.AddSingleton<IPatientsRequestHandler, PatientsRequestHandler>();
            services.Configure<OktaConfiguration>(Configuration.GetSection(OktaConfigSection));
            services.Configure<WebDriverSettings>(Configuration.GetSection(DriverSection));
            services.AddSingleton<IOktaTokenGenerator, OktaTokenGenerator>();
            services.AddTransient<IWebDriverBuilder, WebDriverBuilder>();
            services.AddSingleton<ILoggerBuilder, LoggerBuilder>();
            services.AddSingleton<ITestOutputSink, TestOutputSink>();

            // EF Core
            services.AddSingleton<DataEntryDbContextConfiguration>();
            services.AddSingleton<HealthSystemsDbContextConfiguration>();
            services.AddSingleton<ReportingDbContextConfiguration>();
            services.AddSingleton<RiskDbContextConfiguration>();
            services.AddSingleton<SalesDbContextConfiguration>();

            services.AddSingleton<IDataEntryRepository, DataEntryRepository>();
            services.AddSingleton<IHealthSystemsRepository, HealthSystemsRepository>();
            services.AddSingleton<IReportingRepository, ReportingRepository>();
            services.AddSingleton<IRiskRepository, RiskRepository>();
            services.AddSingleton<ISalesRepository, SalesRepository>();

            services.AddDbContext<DataEntryContext>();
            services.AddDbContext<HealthSystemsContext>();
            services.AddDbContext<ReportingContext>();
            services.AddDbContext<RiskContext>();
            services.AddDbContext<SalesContext>();

            Services = services.BuildServiceProvider();
        }
    }
}
