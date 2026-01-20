using VaxCare.Core;
using Xunit.Abstractions;

namespace VaxCare.Tests.DataEntry
{
    public class TokenGen(ITestOutputHelper output) : BaseTest(output)
    {
        [Theory(Skip = "This is for local token testing or to generate one")]
        [InlineData("Token test")]
        public async Task TokenGenTest(string test)
        {
            await RunTestAsync(test, async () =>
            {
            });
        }
    }
}
