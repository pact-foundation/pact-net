using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpStatusCodeComparer : IHttpStatusCodeComparer
    {
        public ComparisonResult Compare(int expected, int actual)
        {
            var result = new ComparisonResult("has status code {0}", expected);
            if (!expected.Equals(actual))
            {
                result.RecordFailure(new DiffComparisonFailure(expected, actual));
            }

            return result;
        }
    }
}