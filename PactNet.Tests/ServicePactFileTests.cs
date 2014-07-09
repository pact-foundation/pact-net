using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests
{
    public class ServicePactFileTests
    {
        [Fact]
        public void Interactions_WithNoInteractions_ReturnsNull()
        {
            var pactFile = new ServicePactFile();

            Assert.Null(pactFile.Interactions);
        }
    }
}
