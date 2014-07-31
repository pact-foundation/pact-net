using System;
using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public class MockContextService : IMockContextService
    {
        private readonly Func<IEnumerable<KeyValuePair<ProviderServiceRequest, ProviderServiceResponse>>> _requestResponsePairsFactory;

        public MockContextService(Func<IEnumerable<KeyValuePair<ProviderServiceRequest, ProviderServiceResponse>>> requestResponsePairsFactory)
        {
            _requestResponsePairsFactory = requestResponsePairsFactory;
        }

        public IEnumerable<KeyValuePair<ProviderServiceRequest, ProviderServiceResponse>> GetExpectedRequestResponsePairs()
        {
            return _requestResponsePairsFactory();
        }
    }
}