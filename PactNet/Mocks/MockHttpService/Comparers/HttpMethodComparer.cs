using PactNet.Comparers;
using PactNet.Models.ProviderService;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpMethodComparer : IHttpMethodComparer
    {
        public ComparisonResult Compare(HttpVerb expected, HttpVerb actual)
        {
            var result = new ComparisonResult("has method {0}", expected);

            if (!expected.Equals(actual))
            {
                result.RecordFailure(new DiffComparisonFailure(expected, actual));
                return result;
            }

            return result;
        }
    }
}