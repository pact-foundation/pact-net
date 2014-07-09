using System.Net.Http;
using PactNet.Models;

namespace PactNet.Mappers
{
    public interface IHttpMethodMapper
    {
        HttpMethod Convert(HttpVerb from);
    }
}