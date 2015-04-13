using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpPathComparer : IHttpPathComparer
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
                result.RecordFailure(new DiffComparisonFailure(expected, actual));
                return result;
            }

            return result;
        }
    }
}