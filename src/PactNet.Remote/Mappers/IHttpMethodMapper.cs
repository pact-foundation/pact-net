using System.Net.Http;
using PactNet.Remote.Models;

namespace PactNet.Remote.Mappers
{
    internal interface IHttpMethodMapper : IMapper<HttpVerb, HttpMethod>
    {
    }
}