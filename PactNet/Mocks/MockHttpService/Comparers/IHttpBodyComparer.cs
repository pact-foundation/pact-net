using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal interface IHttpBodyComparer
    {
        ComparisonResult Compare(dynamic expected, dynamic actual, bool allowExtraKeys);
    }
}