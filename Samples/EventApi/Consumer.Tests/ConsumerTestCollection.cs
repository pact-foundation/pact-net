using Xunit;

namespace Consumer.Tests
{
    [CollectionDefinition("Consumer test collection")]
    public class ConsumerTestCollection : ICollectionFixture<ConsumerEventApiPact>
    {
    }
}
