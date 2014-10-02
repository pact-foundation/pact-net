using Nancy;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public class MockProviderRequestHandler : IMockProviderRequestHandler
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

            var matchingInteraction = _mockProviderRepository.GetMatchingTestScopedInteraction(actualRequest.Method, actualRequest.Path);

            _mockProviderRepository.AddHandledRequest(new HandledRequest(actualRequest, matchingInteraction));

            return _responseMapper.Convert(matchingInteraction.Response);
        }
    }
}