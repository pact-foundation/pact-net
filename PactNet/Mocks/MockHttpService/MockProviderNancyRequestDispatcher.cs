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

            if (_expectedRequest == null)
            {
                throw new InvalidOperationException("expected request has not been set, please supply using the MockProviderNancyRequestDispatcher.Set static method.");
            }

            if (_expectedResponse == null)
            {
                throw new InvalidOperationException("expected response has not been set, please supply using the MockProviderNancyRequestDispatcher.Set static method.");
            }

            var tcs = new TaskCompletionSource<Response>();

            try
            {
                var response = HandleRequest(context.Request);
                context.Response = response;
                tcs.SetResult(context.Response);
            }
            catch (Exception ex)
            {
                context.Response = null;
                tcs.SetException(ex);
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