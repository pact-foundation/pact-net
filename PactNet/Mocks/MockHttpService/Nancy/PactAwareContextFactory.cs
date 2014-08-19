using Nancy;
using Nancy.Culture;
using Nancy.Diagnostics;
using Nancy.Localization;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public class PactAwareContextFactory : INancyContextFactory
    {
        private readonly IMockContextService _mockContextService;
        private readonly ICultureService _cultureService;
        private readonly IRequestTraceFactory _requestTraceFactory;
        private readonly ITextResource _textResource;

        public PactAwareContextFactory(IMockContextService mockContextService, ICultureService cultureService, IRequestTraceFactory requestTraceFactory, ITextResource textResource)
        {
            _mockContextService = mockContextService;
            _cultureService = cultureService;
            _requestTraceFactory = requestTraceFactory;
            _textResource = textResource;
        }

        public NancyContext Create(Request request)
        {
            var nancyContext = new NancyContext
            {
                Request = request, 
                Trace = _requestTraceFactory.Create(request)
            };

            nancyContext.Culture = _cultureService.DetermineCurrentCulture(nancyContext);
            nancyContext.Text = new TextResourceFinder(_textResource, nancyContext);

            nancyContext.SetMockInteraction(_mockContextService.GetExpectedRequestResponsePairs());

            return nancyContext;
        }
    }
}