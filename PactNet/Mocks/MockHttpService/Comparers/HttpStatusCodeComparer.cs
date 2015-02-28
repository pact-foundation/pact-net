using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpStatusCodeComparer : IHttpStatusCodeComparer
    {
        public ComparisonResult Compare(int expected, int actual)
        {
            var result = new ComparisonResult("has status code {0}", expected);
            if (!expected.Equals(actual))
            {
                result.RecordFailure(expected: expected, actual: actual);
            }

            return result;
        }
    }
}