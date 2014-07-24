using System;
using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public class MockContextService : IMockContextService
    {
        private readonly Func<IEnumerable<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>> _requestResponsePairsFactory;

        public MockContextService(Func<IEnumerable<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>> requestResponsePairsFactory)
        {
            _requestResponsePairsFactory = requestResponsePairsFactory;
        }

        public IEnumerable<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>> GetExpectedRequestResponsePairs()
        {
            return _requestResponsePairsFactory();
        }
    }
}