using Xunit;

namespace PactNet.Tests.IntegrationTests
{
    [CollectionDefinition("Integration test collection")]
    public class IntegrationTestCollection : ICollectionFixture<IntegrationTestsMyApiPact>
    {
    }
}
