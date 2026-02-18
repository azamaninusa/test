using CommandLine;

namespace NightlyBillingTestRunner
{
    public class RunOptions
    {
        [Option(shortName: 'e', longName: "Environment", Required = true, HelpText = "Environment where test should be run")]
        public string? Environment { get; set; }
    }
}
