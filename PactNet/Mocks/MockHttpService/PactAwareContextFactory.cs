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
            return new NancyContext()
            {
                Request = request,
                Parameters = { 
                    ExpectedRequest = _mockContextService.GetExpectedRequest(), 
                    ExpectedResponse = _mockContextService.GetExpectedResponse() 
                }
            };
        }
    }
}