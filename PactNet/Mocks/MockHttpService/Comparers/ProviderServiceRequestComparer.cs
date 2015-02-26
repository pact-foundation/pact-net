using System.Linq;
using PactNet.Comparers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class ProviderServiceRequestComparer : IProviderServiceRequestComparer
    {
        private readonly IHttpMethodComparer _httpMethodComparer;
        private readonly IHttpPathComparer _httpPathComparer;
        private readonly IHttpQueryStringComparer _httpQueryStringComparer;
        private readonly IHttpHeaderComparer _httpHeaderComparer;
        private readonly IHttpBodyComparer _httpBodyComparer;

        private const string MessagePrefix = "\t- Request";

        public ProviderServiceRequestComparer()
        {
            _httpMethodComparer = new HttpMethodComparer(MessagePrefix);
            _httpPathComparer = new HttpPathComparer(MessagePrefix);
            _httpQueryStringComparer = new HttpQueryStringComparer(MessagePrefix);
            _httpHeaderComparer = new HttpHeaderComparer();
            _httpBodyComparer = new HttpBodyComparer();
        }

        public ComparisonResult Compare(ProviderServiceRequest expected, ProviderServiceRequest actual)
        {
            var result = new ComparisonResult();

            if (expected == null)
            {
                result.AddError("Expected request cannot be null");
                return result;
            }

            var methodResult = _httpMethodComparer.Compare(expected.Method, actual.Method);
            result.AddComparisonResult(methodResult);

            var pathResult = _httpPathComparer.Compare(expected.Path, actual.Path);
            result.AddComparisonResult(pathResult);

            var queryResult = _httpQueryStringComparer.Compare(expected.Query, actual.Query);
            result.AddComparisonResult(queryResult);

            if (expected.Headers != null && expected.Headers.Any())
            {
                var headerResult = _httpHeaderComparer.Compare(expected.Headers, actual.Headers);
                result.AddComparisonResult(headerResult);
            }

            if (expected.Body != null)
            {
                var bodyResult = _httpBodyComparer.Compare(expected.Body, actual.Body, true);
                result.AddComparisonResult(bodyResult);
            }

            return result;
        }
    }
}