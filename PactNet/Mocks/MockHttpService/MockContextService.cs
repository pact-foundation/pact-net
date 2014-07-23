using System;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public class MockContextService : IMockContextService
    {
        private readonly Func<PactProviderServiceRequest> _getPactRequest;
        private readonly Func<PactProviderServiceResponse> _getPactResponse;

        public MockContextService(Func<PactProviderServiceRequest> getPactRequest, Func<PactProviderServiceResponse> getPactResponse)
        {
            _getPactRequest = getPactRequest;
            _getPactResponse = getPactResponse;
        }

        public PactProviderServiceRequest GetExpectedRequest()
        {
            return _getPactRequest();
        }

        public PactProviderServiceResponse GetExpectedResponse()
        {
            return _getPactResponse();
        }
    }
}