using System;
using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public class MockContextService : IMockContextService
    {
        private readonly Func<IEnumerable<ProviderServiceInteraction>> _requestResponsePairsFactory;

        public MockContextService(Func<IEnumerable<ProviderServiceInteraction>> requestResponsePairsFactory)
        {
            _requestResponsePairsFactory = requestResponsePairsFactory;
        }

        public IEnumerable<ProviderServiceInteraction> GetExpectedRequestResponsePairs()
        {
            return _requestResponsePairsFactory();
        }
    }
}