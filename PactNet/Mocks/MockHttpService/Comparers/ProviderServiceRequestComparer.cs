using System.Linq;
using PactNet.Comparers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class ProviderServiceRequestComparer : IProviderServiceRequestComparer
    {
        private readonly IHttpMethodComparer _httpMethodComparer;
        private readonly IHttpPathComparer _httpPathComparer;
        private readonly IHttpQueryStringComparer _httpQueryStringComparer;
        private readonly IHttpHeaderComparer _httpHeaderComparer;
        private readonly IHttpBodyComparer _httpBodyComparer;

        public ProviderServiceRequestComparer()
        {
            _httpMethodComparer = new HttpMethodComparer();
            _httpPathComparer = new HttpPathComparer();
            _httpQueryStringComparer = new HttpQueryStringComparer();
            _httpHeaderComparer = new HttpHeaderComparer();
            _httpBodyComparer = new HttpBodyComparer();
        }

        public ComparisonResult Compare(ProviderServiceRequest expected, ProviderServiceRequest actual)
        {
            var result = new ComparisonResult("sends a request which");

            if (expected == null)
            {
                result.RecordFailure(new ErrorMessageComparisonFailure("Expected request cannot be null"));
                return result;
            }

            var methodResult = _httpMethodComparer.Compare(expected.Method, actual.Method);
            result.AddChildResult(methodResult);

            var pathResult = _httpPathComparer.Compare(expected.Path, actual.Path);
            result.AddChildResult(pathResult);

            var queryResult = _httpQueryStringComparer.Compare(expected.Query, actual.Query);
            result.AddChildResult(queryResult);

            if (expected.Headers != null && expected.Headers.Any())
            {
                var headerResult = _httpHeaderComparer.Compare(expected.Headers, actual.Headers);
                result.AddChildResult(headerResult);
            }

            if (expected.Body != null)
            {
                var bodyResult = _httpBodyComparer.Compare(expected.Body, actual.Body, true);
                result.AddChildResult(bodyResult);
            }

            return result;
        }
    }
}