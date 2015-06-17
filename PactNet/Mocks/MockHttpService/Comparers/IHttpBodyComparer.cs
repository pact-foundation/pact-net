using System.Collections.Generic;
using PactNet.Comparers;
using PactNet.Matchers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal interface IHttpBodyComparer
    {
        ComparisonResult Compare(dynamic expected, dynamic actual, IEnumerable<IMatcher> matchingRules);
    }
}