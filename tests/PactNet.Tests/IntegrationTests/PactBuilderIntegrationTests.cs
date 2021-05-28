using System;
using PactNet.Mocks.MockHttpService;
using Xunit;

namespace PactNet.Tests.IntegrationTests
{
    public class PactBuilderIntegrationTests : IClassFixture<IntegrationTestsMyApiPact>
    {
        private readonly IMockProviderService _mockProviderService;
        private readonly Uri _mockProviderServiceBaseUri;

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
