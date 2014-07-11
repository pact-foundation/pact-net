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
        private readonly IEncodingMapper _encodingMapper;

        [Obsolete("For testing only.")]
        public HttpRequestMessageMapper(
            IHttpMethodMapper httpMethodMapper,
            IHttpContentMapper httpContentMapper,
            IEncodingMapper encodingMapper)
        {
            _httpMethodMapper = httpMethodMapper;
            _httpContentMapper = httpContentMapper;
            _encodingMapper = encodingMapper;
        }

        public HttpRequestMessageMapper() : this(
            new HttpMethodMapper(),
            new HttpContentMapper(),
            new EncodingMapper())
        {
        }

        public HttpRequestMessage Convert(PactProviderServiceRequest from)
        {
            if (from == null)
            {
                return null;
            }

            string contentType = null;
            Encoding encoding = null;

            var requestHttpMethod = _httpMethodMapper.Convert(from.Method);
            var requestPath = from.PathWithQuery();

            var to = new HttpRequestMessage(requestHttpMethod, requestPath);

            if (from.Headers != null && from.Headers.Any())
            {
                foreach (var requestHeader in from.Headers)
                {
                    //TODO: Check if there are any other headers which need special treatment
                    //Handle the content-type header as little differently, as they need to be attached to the content when using a HttpRequestMessage
                    if (requestHeader.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var contentTypeHeaderSplit = requestHeader.Value.Split(';');

                        contentType = contentTypeHeaderSplit.First().Trim();

                        var encodingString = contentTypeHeaderSplit.FirstOrDefault(x => x.Contains("charset="));
                        if (!String.IsNullOrEmpty(encodingString))
                        {
                            encodingString = encodingString.Trim().Replace("charset=", "");
                            encoding = _encodingMapper.Convert(encodingString);
                        }
                        continue;
                    }

                    //Strip the Content-Length header as is automatically attached to the request
                    if (requestHeader.Key.Equals("Content-Length", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    to.Headers.Add(requestHeader.Key, requestHeader.Value);
                }
            }

            to.Content = _httpContentMapper.Convert(from.Body, encoding, contentType);

            return to;
        }
    }
}
