using System;
using System.Linq;
using System.Net.Http;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public class HttpRequestMessageMapper : IHttpRequestMessageMapper
    {
        private readonly IHttpMethodMapper _httpMethodMapper;
        private readonly IHttpContentMapper _httpContentMapper;
        private readonly IHttpBodyContentMapper _httpBodyContentMapper;

        internal HttpRequestMessageMapper(
            IHttpMethodMapper httpMethodMapper,
            IHttpContentMapper httpContentMapper,
            IHttpBodyContentMapper httpBodyContentMapper)
        {
            _httpMethodMapper = httpMethodMapper;
            _httpContentMapper = httpContentMapper;
            _httpBodyContentMapper = httpBodyContentMapper;
        }

        public HttpRequestMessageMapper() : this(
            new HttpMethodMapper(),
            new HttpContentMapper(),
            new HttpBodyContentMapper())
        {
        }

        public HttpRequestMessage Convert(ProviderServiceRequest from)
        {
            if (from == null)
            {
                return null;
            }

            var requestHttpMethod = _httpMethodMapper.Convert(from.Method);
            var requestPath = from.PathWithQuery();

            var to = new HttpRequestMessage(requestHttpMethod, requestPath);

            if (from.Headers != null && from.Headers.Any())
            {
                foreach (var requestHeader in from.Headers)
                {
                    //TODO: Check if there are any other headers which need special treatment
                    //Handle the content-type header as little differently, as they need to be attached to the content when using a HttpRequestMessage
                    //Strip the Content-Length header as is automatically attached to the request
                    if (requestHeader.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase) || 
                        requestHeader.Key.Equals("Content-Length", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    to.Headers.Add(requestHeader.Key, requestHeader.Value);
                }
            }

            if (from.Body != null)
            {
                HttpBodyContent bodyContent = _httpBodyContentMapper.Convert(from.Body, from.Headers);
                to.Content = _httpContentMapper.Convert(bodyContent);
            }

            return to;
        }
    }
}
