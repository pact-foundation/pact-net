using System.Linq;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Reporters;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class ProviderServiceRequestComparer : IProviderServiceRequestComparer
    {
        private readonly IHttpMethodComparer _httpMethodComparer;
        private readonly IHttpPathComparer _httpPathComparer;
        private readonly IHttpQueryStringComparer _httpQueryStringComparer;
        private readonly IHttpHeaderComparer _httpHeaderComparer;
        private readonly IHttpBodyComparer _httpBodyComparer;
        private readonly IReporter _reporter;

        private const string MessagePrefix = "\t- Request";

        public ProviderServiceRequestComparer(IReporter reporter)
        {
            _reporter = reporter;
            _httpMethodComparer = new HttpMethodComparer(MessagePrefix, _reporter);
            _httpPathComparer = new HttpPathComparer(MessagePrefix, _reporter);
            _httpQueryStringComparer = new HttpQueryStringComparer(MessagePrefix, _reporter);
            _httpHeaderComparer = new HttpHeaderComparer(MessagePrefix, _reporter);
            _httpBodyComparer = new HttpBodyComparer(MessagePrefix, _reporter);
        }

        public void Compare(ProviderServiceRequest expected, ProviderServiceRequest actual)
        {
            if (expected == null)
            {
                _reporter.ReportError("Expected request cannot be null");
                return;
            }

            _httpMethodComparer.Compare(expected.Method, actual.Method);

            _httpPathComparer.Compare(expected.Path, actual.Path);

            _httpQueryStringComparer.Compare(expected.Query, actual.Query);

            if (expected.Headers != null && expected.Headers.Any())
            {
                if (actual.Headers == null)
                {
                    _reporter.ReportError("Headers are null");
                }

                _httpHeaderComparer.Compare(expected.Headers, actual.Headers);
            }

            if (expected.Body != null)
            {
                _httpBodyComparer.Validate(expected.Body, actual.Body, true);
            }
        }
    }
}