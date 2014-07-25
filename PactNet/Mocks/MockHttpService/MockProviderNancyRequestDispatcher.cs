using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Routing;

namespace PactNet.Mocks.MockHttpService
{
    public class MockProviderNancyRequestDispatcher : IRequestDispatcher
    {
        private readonly MockNancyRequestHandler _mockNancyRequestHandler;

        public MockProviderNancyRequestDispatcher(MockNancyRequestHandler _mockNancyRequestHandler)
        {
            this._mockNancyRequestHandler = _mockNancyRequestHandler;
        }

        public Task<Response> Dispatch(NancyContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            return _mockNancyRequestHandler.Handle(context);
        }
    }
}