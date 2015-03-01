using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpPathComparer : IHttpPathComparer
    {
        public ComparisonResult Compare(string expected, string actual)
        {
            var result = new ComparisonResult("has path {0}", expected);

            if (expected == null)
            {
                return result;
            }

            if (!expected.Equals(actual))
            {
                result.RecordFailure(expected, actual);
                return result;
            }

            return result;
        }
    }
}