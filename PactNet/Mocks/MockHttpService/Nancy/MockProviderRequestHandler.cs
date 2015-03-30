using System;
using Nancy;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    internal class MockProviderRequestHandler : IMockProviderRequestHandler
    {
        private readonly INancyResponseMapper _responseMapper;
        private readonly IProviderServiceRequestMapper _requestMapper;
        private readonly IMockProviderRepository _mockProviderRepository;

        public MockProviderRequestHandler(
            IProviderServiceRequestMapper requestMapper,
            INancyResponseMapper responseMapper,
            IMockProviderRepository mockProviderRepository)
        {
            _requestMapper = requestMapper;
            _responseMapper = responseMapper;
            _mockProviderRepository = mockProviderRepository;
        }

        public Response Handle(NancyContext context)
        {
            return HandlePactRequest(context);
        }

        private Response HandlePactRequest(NancyContext context)
        {
            var actualRequest = _requestMapper.Convert(context.Request);

            ProviderServiceInteraction matchingInteraction;
            
            try
            {
                matchingInteraction = _mockProviderRepository.GetMatchingTestScopedInteraction(actualRequest);
                _mockProviderRepository.AddHandledRequest(new HandledRequest(actualRequest, matchingInteraction));
            }
            catch (Exception)
            {
                _mockProviderRepository.AddHandledRequest(new HandledRequest(actualRequest, null));
                throw;
            }
            
            return _responseMapper.Convert(matchingInteraction.Response);
        }
    }
}