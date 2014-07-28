using System;
using Nancy;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public class MockProviderNancyRequestHandler : IMockProviderNancyRequestHandler
    {
        private readonly INancyResponseMapper _responseMapper;
        private readonly IPactProviderServiceRequestComparer _requestComparer;
        private readonly IPactProviderServiceRequestMapper _requestMapper;

        public MockProviderNancyRequestHandler(IPactProviderServiceRequestComparer requestComparer, 
            IPactProviderServiceRequestMapper requestMapper, 
            INancyResponseMapper responseMapper)
        {
            _requestComparer = requestComparer;
            _requestMapper = requestMapper;
            _responseMapper = responseMapper;
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
                var errorResponse = new PactProviderServiceResponse
                {
                    Status = 500,
                    Body = new
                    {
                        ErrorMessage = ex.Message,
                        ex.StackTrace
                    }
                };
                var response = _responseMapper.Convert(errorResponse);
                response.ReasonPhrase = ex.Message;

                return response;
            }
        }

        private Response HandlePactRequest(NancyContext context)
        {
            var actualRequest = _requestMapper.Convert(context.Request);

            var matchingRequestResponsePair = context.GetMatchingMockRequestResponsePair(actualRequest.Method, actualRequest.Path);
            var expectedRequest = matchingRequestResponsePair.Key;
            var expectedResponse = matchingRequestResponsePair.Value;

            //TODO:NC Check if this is still required (will write a test for this)
            // it can be removed once the logic in NancyExtensions are unit tested (PK)
            if (expectedRequest == null)
            {
                throw new InvalidOperationException("Expected request has not been set.");
            }

            if (expectedResponse == null)
            {
                throw new InvalidOperationException("Expected response has not been set.");
            }

            _requestComparer.Compare(expectedRequest, actualRequest);

            return _responseMapper.Convert(expectedResponse);
        }
    }
}