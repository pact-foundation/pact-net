using System.Collections.Generic;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    using PactNet.Comparers;

    public interface IHttpHeaderComparer : IComparer<IDictionary<string, string>>
    {
    }
}