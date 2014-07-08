using System.Net.Http;
using System.Text;

namespace PactNet.Mappers
{
    public interface IHttpContentMapper
    {
        HttpContent Convert(dynamic from, Encoding encoding, string contentType);
    }
}