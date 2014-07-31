using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests
{
    public class ProviderServicePactFileTests
    {
        [Fact]
        public void Interactions_WithNoInteractions_ReturnsNull()
        {
            var pactFile = new ProviderServicePactFile();

            Assert.Null(pactFile.Interactions);
        }
    }
}
