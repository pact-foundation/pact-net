using PactNet.Models;
using Xunit;

namespace PactNet.Tests.Models
{
    public class PactDetailsTests
    {
        [Fact]
        public void GeneratePactFileName_WhenConsumerNameAndProviderNameHaveBeenSet_ReturnsFileName()
        {
            var details = new PactDetails
            {
                Consumer = new Party { Name = "My Consumer" },
                Provider = new Party { Name = "My Provider" }
            };

            var fileName = details.GeneratePactFileName();

            Assert.Equal("my_consumer-my_provider.json", fileName);
        }

        [Fact]
        public void GeneratePactFileName_WhenConsumerNameAndProviderNameHaveNotBeenSet_ReturnsFileNameWithNoConsumerAndProviderNameAndDoesNotThrow()
        {
            var details = new PactDetails
            {
                Consumer = new Party(),
                Provider = new Party()
            };

            var fileName = details.GeneratePactFileName();

            Assert.Equal("-.json", fileName);
        }

        [Fact]
        public void GeneratePactFileName_WhenConsumerAndProviderHaveNotBeenSet_ReturnsFileNameWithNoConsumerAndProviderNameAndDoesNotThrow()
        {
            var details = new PactDetails();

            var fileName = details.GeneratePactFileName();

            Assert.Equal("-.json", fileName);
        }
    }
}
