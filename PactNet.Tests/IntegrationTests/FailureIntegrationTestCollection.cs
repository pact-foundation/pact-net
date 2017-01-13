using Xunit;

namespace PactNet.Tests.IntegrationTests
{
    [CollectionDefinition("Failure integration test collection")]
    public class FailureIntegrationTestCollection : ICollectionFixture<FailureIntegrationTestsMyApiPact>
    {
    }
}
