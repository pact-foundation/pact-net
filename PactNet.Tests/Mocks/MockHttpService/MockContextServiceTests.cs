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
            var requestResponsePairs = new List<KeyValuePair<ProviderServiceRequest, ProviderServiceResponse>>
            {
                new KeyValuePair<ProviderServiceRequest, ProviderServiceResponse>(new ProviderServiceRequest { Method = HttpVerb.Get, Path = "/events" }, new ProviderServiceResponse()),
                new KeyValuePair<ProviderServiceRequest, ProviderServiceResponse>(new ProviderServiceRequest { Method = HttpVerb.Post, Path = "/events" }, new ProviderServiceResponse()),
            };

            IMockContextService mockContextService = new MockContextService(() => requestResponsePairs);

            var result = mockContextService.GetExpectedRequestResponsePairs();

            Assert.Equal(requestResponsePairs, result);
        }
    }
}
