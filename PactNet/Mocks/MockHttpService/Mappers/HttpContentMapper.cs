using System.Net.Http;
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

            stringContent.Headers.ContentType = from.ContentType;
            stringContent.Headers.ContentType.CharSet = from.Encoding.WebName;

            return stringContent;
        }
    }
}