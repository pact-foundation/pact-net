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
        private readonly IStatsProvider _statsProvider;

        public MockProviderRequestHandler(
            IProviderServiceRequestMapper requestMapper,
            INancyResponseMapper responseMapper,
            IStatsProvider statsProvider)
        {
            _requestMapper = requestMapper;
            _responseMapper = responseMapper;
            _statsProvider = statsProvider;
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

            _statsProvider.AddStat(new Stat(actualRequest, matchingInteraction));

            matchingInteraction.IncrementUsage(); //TODO: Remove this as well!

            return _responseMapper.Convert(matchingInteraction.Response);
        }
    }
}