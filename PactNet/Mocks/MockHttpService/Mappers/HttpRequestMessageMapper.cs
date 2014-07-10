using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public class HttpRequestMessageMapper : IHttpRequestMessageMapper
    {
        private readonly IHttpMethodMapper _httpMethodMapper;
        private readonly IHttpContentMapper _httpContentMapper;

        public HttpRequestMessageMapper(
            IHttpMethodMapper httpMethodMapper,
            IHttpContentMapper httpContentMapper)
        {
            _httpMethodMapper = httpMethodMapper;
            _httpContentMapper = httpContentMapper;
        }

        public HttpRequestMessageMapper() : this(
            new HttpMethodMapper(),
            new HttpContentMapper())
        {
        }

        public HttpRequestMessage Convert(PactServiceInteraction from)
        {
            if (from == null)
            {
                return null;
            }

            string contentType = null;
            Encoding encoding = null; //TODO: Handle request encoding and charset

            //Map headers
            var to = new HttpRequestMessage(_httpMethodMapper.Convert(from.Request.Method), from.Request.PathWithQuery());

            if (from.Request.Headers != null && from.Request.Headers.Any())
            {
                foreach (var requestHeader in from.Request.Headers)
                {
                    //TODO: Check if there are any other headers which need special treatment
                    //Handle the content-type header as little differently, as they need to be attached to the content when using a HttpRequestMessage
                    if (requestHeader.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var contentTypeHeaderSplit = requestHeader.Value.Split(';');

                        contentType = contentTypeHeaderSplit.First();
                        continue;
                    }

                    to.Headers.Add(requestHeader.Key, requestHeader.Value);
                }
            }

            to.Content = _httpContentMapper.Convert(from.Request.Body, encoding, contentType);

            return to;
        }
    }
}
