using PactNet.Mocks.MockHttpService;
using Xunit;

namespace PactNet.Tests.IntegrationTests
{
    [Collection("Integration test collection")]
    public class PactBuilderIntegrationTests
    {
        private IMockProviderService _mockProviderService;
        private string _mockProviderServiceBaseUri;

        public PactBuilderIntegrationTests(IntegrationTestsMyApiPact data)
        {
            _mockProviderService = data.MockProviderService;
            _mockProviderServiceBaseUri = data.MockProviderServiceBaseUri;
            _mockProviderService.ClearInteractions();
        }

        [Fact]
        public void WhenNotRegisteringAnyInteractions_VerificationSucceeds()
        {
            _mockProviderService.VerifyInteractions();
        }
    }
}
