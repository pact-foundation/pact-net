using System.Net.Http;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public class HttpContentMapper : IHttpContentMapper
    {
        public HttpContent Convert(HttpBodyContent from)
        {
            if (from == null)
            {
                return null;
            }

            return new StringContent(from.Content, from.Encoding, from.ContentType);
        }
    }
}