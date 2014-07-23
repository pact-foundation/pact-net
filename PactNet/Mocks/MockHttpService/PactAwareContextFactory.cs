using System.Dynamic;
using Nancy;

namespace PactNet.Mocks.MockHttpService
{
    public class PactAwareContextFactory : INancyContextFactory
    {
        private readonly IMockContextService _mockContextService;

        public PactAwareContextFactory(IMockContextService mockContextService)
        {
            _mockContextService = mockContextService;
        }

        public NancyContext Create(Request request)
        {
            var nancyContext = new NancyContext()
            {
                Request = request,
                Parameters = new ExpandoObject()
            };
            nancyContext.Parameters.ExpectedRequest = _mockContextService.GetExpectedRequest();
            nancyContext.Parameters.ExpectedResponse = _mockContextService.GetExpectedResponse();

            return nancyContext;
        }
    }
}