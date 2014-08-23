using System;
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
            try
            {
                var response = HandlePactRequest(context);

                return response;
            }
            catch (Exception ex)
            {
                var exceptionMessage = ex.Message
                    .Replace("\r", " ")
                    .Replace("\n", "")
                    .Replace("\t", " ")
                    .Replace(@"\", "");

                var errorResponse = new ProviderServiceResponse
                {
                    Status = 500,
                    Body = exceptionMessage
                };
                var response = _responseMapper.Convert(errorResponse);
                response.ReasonPhrase = exceptionMessage;

                return response;
            }
        }

        private Response HandlePactRequest(NancyContext context)
        {
            var actualRequest = _requestMapper.Convert(context.Request);

            var matchingInteraction = context.GetMatchingInteraction(actualRequest.Method, actualRequest.Path);

            _mockProviderRepository.AddHandledRequest(new HandledRequest(actualRequest, matchingInteraction));

            matchingInteraction.IncrementUsage(); //TODO: Eventually we want to remove this

            return _responseMapper.Convert(matchingInteraction.Response);
        }
    }
}