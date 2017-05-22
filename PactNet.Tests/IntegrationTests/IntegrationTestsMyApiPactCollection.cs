using Xunit;

namespace PactNet.Tests.IntegrationTests
{
    [CollectionDefinition("IntegrationTestsMyApiPactCollection")]
    public class IntegrationTestsMyApiPactCollection : ICollectionFixture<IntegrationTestsMyApiPact>
    {
    }
}