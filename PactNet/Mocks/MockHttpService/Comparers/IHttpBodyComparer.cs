using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public interface IHttpBodyComparer
    {
        ComparisonResult Compare(dynamic expected, dynamic actual, bool useStrict = false);
    }
}