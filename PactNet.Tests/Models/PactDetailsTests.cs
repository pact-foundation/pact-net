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
                Consumer = new Pacticipant { Name = "My Consumer" },
                Provider = new Pacticipant { Name = "My Provider" }
            };

            var fileName = details.GeneratePactFileName();

            Assert.Equal("my_consumer-my_provider.json", fileName);
        }

        [Fact]
        public void GeneratePactFileName_WhenConsumerNameAndProviderNameHaveNotBeenSet_ReturnsFileNameWithNoConsumerAndProviderNameAndDoesNotThrow()
        {
            var details = new PactDetails
            {
                Consumer = new Pacticipant(),
                Provider = new Pacticipant()
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
