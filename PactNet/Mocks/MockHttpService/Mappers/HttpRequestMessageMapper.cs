using System;
using System.Collections.Generic;
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

            var contentRelatedHeaders = new Dictionary<string, string>();
            if (from.Headers != null && from.Headers.Any())
            {
                foreach (var requestHeader in from.Headers)
                {
                    //Strip any Content- headers as they need to be attached to Request content when using a HttpRequestMessage
                    if (requestHeader.Key.IndexOf("Content-", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        contentRelatedHeaders.Add(requestHeader.Key, requestHeader.Value);
                        continue;
                    }

                    to.Headers.Add(requestHeader.Key, requestHeader.Value);
                }
            }

            if (from.Body != null)
            {
                HttpBodyContent bodyContent = _httpBodyContentMapper.Convert(from.Body, from.Headers);
                var httpContent = _httpContentMapper.Convert(bodyContent);

                //Set the content related headers
                if (httpContent != null && contentRelatedHeaders.Any())
                {
                    foreach (var contentHeader in contentRelatedHeaders)
                    {
                        if (contentHeader.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase) && 
                            httpContent.Headers.Any(x => x.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            continue;
                        }

                        httpContent.Headers.Add(contentHeader.Key, contentHeader.Value);
                    }
                }

                to.Content = httpContent;
            }

            return to;
        }
    }
}
