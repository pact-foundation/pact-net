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
            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest { Method = HttpVerb.Get, Path = "/events" }, new PactProviderServiceResponse()),
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest { Method = HttpVerb.Post, Path = "/events" }, new PactProviderServiceResponse()),
            };

            IMockContextService mockContextService = new MockContextService(() => requestResponsePairs);

            var result = mockContextService.GetExpectedRequestResponsePairs();

            Assert.Equal(requestResponsePairs, result);
        }
    }
}
