using System.Net.Http;

namespace PactNet.Mappers
{
    public interface IHttpRequestMessageMapper
    {
        HttpRequestMessage Convert(PactServiceInteraction from);
    }
}