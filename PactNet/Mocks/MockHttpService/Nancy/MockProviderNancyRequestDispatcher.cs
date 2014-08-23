using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Routing;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public class MockProviderNancyRequestDispatcher : IRequestDispatcher
    {
        private readonly IMockProviderRequestHandler _requestHandler;
        private readonly IMockProviderAdminRequestHandler _adminRequestHandler;

        public MockProviderNancyRequestDispatcher(
            IMockProviderRequestHandler requestHandler,
            IMockProviderAdminRequestHandler adminRequestHandler)
        {
            _requestHandler = requestHandler;
            _adminRequestHandler = adminRequestHandler;
        }

        public Task<Response> Dispatch(NancyContext context, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Response>();

            if (cancellationToken.IsCancellationRequested)
            {
                tcs.SetException(new OperationCanceledException());
                return tcs.Task;
            }

            if (context == null)
            {
                tcs.SetException(new ArgumentException("context is null"));
                return tcs.Task;
            }

            var response = IsAdminRequest(context.Request) ?
                                    _adminRequestHandler.Handle(context) :
                                    _requestHandler.Handle(context);

            context.Response = response;
            tcs.SetResult(context.Response);

            return tcs.Task;
        }

        private bool IsAdminRequest(Request request)
        {
            return request.Headers != null &&
                   request.Headers.Any(x => x.Key == Constants.AdministrativeRequestHeaderKey);
        }
    }
}