using PactNet.Comparers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal interface IHttpMethodComparer : IComparer<HttpVerb>
    {
    }
}