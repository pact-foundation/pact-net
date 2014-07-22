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
    using System.Net;

    public class MockProviderNancyRequestDispatcher : IRequestDispatcher, IDisposable
    {
        private readonly IPactProviderServiceRequestComparer _requestComparer;
        private readonly IPactProviderServiceRequestMapper _requestMapper;
        private readonly INancyResponseMapper _responseMapper;

        private static PactProviderServiceRequest _expectedRequest;
        private static PactProviderServiceResponse _expectedResponse;
        
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

            var tcs = new TaskCompletionSource<Response>();

            try
            {
                if (_expectedRequest == null)
                {
                    throw new InvalidOperationException("Expected request has not been set. Please ensure With() has been called with your expected request and RegisterInteraction() has also called on the Pact.");
                }

                if (_expectedResponse == null)
                {
                    throw new InvalidOperationException("Expected response has not been set. Please ensure WillRespondWith() has been called with your expected response and RegisterInteraction() has also called on the Pact.");
                }

                var response = HandleRequest(context.Request);
                context.Response = response;
                tcs.SetResult(context.Response);
            }
            catch (Exception ex)
            {
                var errorResponse = new PactProviderServiceResponse
                {
                    Status = (HttpStatusCode)500,
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

        public static void Set(PactProviderServiceRequest request, PactProviderServiceResponse response)
        {
            _expectedRequest = request;
            _expectedResponse = response;
        }

        private Response HandleRequest(Request request)
        {
            var actualRequest = _requestMapper.Convert(request);

            _requestComparer.Compare(_expectedRequest, actualRequest);

            return _responseMapper.Convert(_expectedResponse);
        }

        public static void Reset()
        {
            _expectedRequest = null;
            _expectedResponse = null;
        }

        public void Dispose()
        {
            Reset();
        }
    }
}