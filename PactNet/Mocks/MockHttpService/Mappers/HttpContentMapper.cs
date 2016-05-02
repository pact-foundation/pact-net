using System.Net.Http;
using System.Net.Http.Headers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal class HttpContentMapper : IHttpContentMapper
    {
        public HttpContent Convert(HttpBodyContent from)
        {
            if (from == null)
            {
                return null;
            }

            var stringContent = new StringContent(from.Content, from.Encoding);

            stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse(from.ContentType);
            stringContent.Headers.ContentType.CharSet = from.Encoding.WebName;

            return stringContent;
        }
    }
}