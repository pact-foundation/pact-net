using PactNet.Comparers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public interface IHttpMethodComparer : IComparer<HttpVerb>
    {
    }
}