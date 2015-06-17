using System.Linq;
using PactNet.Comparers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class ProviderServiceResponseComparer : IProviderServiceResponseComparer
    {
        private readonly IHttpStatusCodeComparer _httpStatusCodeComparer;
        private readonly IHttpHeaderComparer _httpHeaderComparer;
        private readonly IHttpBodyComparer _httpBodyComparer;

        public ProviderServiceResponseComparer()
        {
            _httpStatusCodeComparer = new HttpStatusCodeComparer();
            _httpHeaderComparer = new HttpHeaderComparer();
            _httpBodyComparer = new HttpBodyComparer();
        }

        public ComparisonResult Compare(ProviderServiceResponse expected, ProviderServiceResponse actual)
        {
            var result = new ComparisonResult("returns a response which");

            if (expected == null)
            {
                result.RecordFailure(new ErrorMessageComparisonFailure("Expected response cannot be null"));
                return result;
            }

            var statusResult = _httpStatusCodeComparer.Compare(expected.Status, actual.Status);
            result.AddChildResult(statusResult);

            if (expected.Headers != null && expected.Headers.Any())
            {
                var headerResult = _httpHeaderComparer.Compare(expected.Headers, actual.Headers);
                result.AddChildResult(headerResult);
            }

            if (expected.Body != null)
            {
                var bodyResult = _httpBodyComparer.Compare(expected.Body, actual.Body, actual.MatchingRules);
                result.AddChildResult(bodyResult);
            }

            return result;
        }
    }
}