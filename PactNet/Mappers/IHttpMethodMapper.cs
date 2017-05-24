using PactNet.Mocks.MockHttpService.Models;
using System.Net.Http;

namespace PactNet.Mappers
{
    public interface IHttpMethodMapper
    {
        HttpMethod Convert(HttpVerb from);
    }
}