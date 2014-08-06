using System.Collections.Generic;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService
{
    public class MockContextServiceTests
    {
        [Fact]
        public void GetExpectedRequestResponsePairs_With_ReturnsRequestResponsePairs()
        {
            var interactions = new List<ProviderServiceInteraction>()
            {
                new ProviderServiceInteraction() { Request = new ProviderServiceRequest { Method = HttpVerb.Get, Path = "/events" }, Response = new ProviderServiceResponse()},
                new ProviderServiceInteraction() { Request = new ProviderServiceRequest { Method = HttpVerb.Post, Path = "/events" }, Response = new ProviderServiceResponse()},
            };

            IMockContextService mockContextService = new MockContextService(() => interactions);

            var result = mockContextService.GetExpectedRequestResponsePairs();

            Assert.Equal(interactions, result);
        }
    }
}
