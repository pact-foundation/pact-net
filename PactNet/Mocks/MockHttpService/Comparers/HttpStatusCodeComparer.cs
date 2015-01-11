using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpStatusCodeComparer : IHttpStatusCodeComparer
    {
        public ComparisonResult Compare(int expected, int actual)
        {
            var result = new ComparisonResult();

            if (!expected.Equals(actual))
            {
                result.AddError(expected: expected, actual: actual);
            }

            return result;
        }
    }
}