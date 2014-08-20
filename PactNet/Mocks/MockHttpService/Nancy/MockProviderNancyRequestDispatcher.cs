using System;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Routing;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public class MockProviderNancyRequestDispatcher : IRequestDispatcher
    {
        private readonly IMockProviderNancyRequestHandler _requestHandler;
        //TODO: Inject a db here
        //TODO: Have a Admin http request, which queries the data
        //TODO: Have an Admin http request, which clears the state

        public MockProviderNancyRequestDispatcher(IMockProviderNancyRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
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

            var response = _requestHandler.Handle(context);

            context.Response = response;
            tcs.SetResult(context.Response);

            return tcs.Task;
        }
    }
}