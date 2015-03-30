using PactNet.Comparers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpMethodComparer : IHttpMethodComparer
    {
        public ComparisonResult Compare(HttpVerb expected, HttpVerb actual)
        {
            var result = new ComparisonResult("has method {0}", expected);

            if (!expected.Equals(actual))
            {
                result.RecordFailure(expected, actual);
                return result;
            }

            return result;
        }
    }
}