using System;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Routing;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public class MockProviderNancyRequestDispatcher : IRequestDispatcher
    {
        private readonly IPactProviderServiceRequestComparer _requestComparer;
        private readonly IPactProviderServiceRequestMapper _requestMapper;
        private readonly INancyResponseMapper _responseMapper;
        
        public MockProviderNancyRequestDispatcher(
            IPactProviderServiceRequestComparer requestComparer,
            IPactProviderServiceRequestMapper requestMapper,
            INancyResponseMapper responseMapper)
        {
            _requestComparer = requestComparer;
            _requestMapper = requestMapper;
            _responseMapper = responseMapper;
        }

        public Task<Response> Dispatch(NancyContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var expectedRequest = context.GetMockRequest();
            var expectedResponse = context.GetMockResponse();

            var tcs = new TaskCompletionSource<Response>();

            try
            {
                if (expectedRequest == null)
                {
                    throw new InvalidOperationException("Expected request has not been set.");
                }

                if (expectedResponse == null)
                {
                    throw new InvalidOperationException("Expected response has not been set.");
                }

                var response = HandleRequest(context.Request, expectedRequest, expectedResponse);
                context.Response = response;
                tcs.SetResult(context.Response);
            }
            catch (Exception ex)
            {
                var errorResponse = new PactProviderServiceResponse
                {
                    Status = 500,
                    Body = new
                    {
                        ErrorMessage = ex.Message,
                        StackTrace = ex.StackTrace
                    }
                };
                var response = _responseMapper.Convert(errorResponse);
                response.ReasonPhrase = ex.Message;

                context.Response = response;
                tcs.SetResult(context.Response);
            }

            return tcs.Task;
        }

        private Response HandleRequest(Request request, PactProviderServiceRequest expectedRequest, PactProviderServiceResponse expectedResponse)
        {
            var actualRequest = _requestMapper.Convert(request);

            _requestComparer.Compare(expectedRequest, actualRequest);

            return _responseMapper.Convert(expectedResponse);
        }
    }
}