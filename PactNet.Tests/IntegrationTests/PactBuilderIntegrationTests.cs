using System;
using PactNet.Mocks.MockHttpService;
using Xunit;

namespace PactNet.Tests.IntegrationTests
{
    public class PactBuilderIntegrationTests : IUseFixture<IntegrationTestsMyApiPact>
    {
        private IMockProviderService _mockProviderService;
        private Uri _mockProviderServiceBaseUri;

        public void SetFixture(IntegrationTestsMyApiPact data)
        {
            _mockProviderService = data.MockProviderService;
            _mockProviderServiceBaseUri = data.MockProviderServiceBaseUri;
            _mockProviderService.ClearInteractions();
        }

        //[Fact(Skip = "Should I Keep")]
        public void WhenNotRegisteringAnyInteractions_VerificationSucceeds()
        {
            _mockProviderService.VerifyInteractions();
        }
    }
}
