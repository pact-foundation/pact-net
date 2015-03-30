using System.Collections.Generic;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    using PactNet.Comparers;

    internal interface IHttpHeaderComparer : IComparer<IDictionary<string, string>>
    {
    }
}