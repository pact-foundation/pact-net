using System.Net.Http;
using System.Text;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public interface IHttpContentMapper
    {
        HttpContent Convert(dynamic from, Encoding encoding, string contentType);
    }
}