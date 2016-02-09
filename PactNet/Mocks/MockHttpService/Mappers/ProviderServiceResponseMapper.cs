using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal class ProviderServiceResponseMapper : IProviderServiceResponseMapper
    {
        private readonly IHttpBodyContentMapper _httpBodyContentMapper;

        internal ProviderServiceResponseMapper(IHttpBodyContentMapper httpBodyContentMapper)
        {
            _httpBodyContentMapper = httpBodyContentMapper;
        }

        public ProviderServiceResponseMapper() : this(
            new HttpBodyContentMapper())
        {
            
        }

        public ProviderServiceResponse Convert(HttpResponseMessage from)
        {
            if (from == null)
            {
                return null;
            }

            var to = new ProviderServiceResponse
            {
                Status = (int) from.StatusCode,
                Headers = ConvertHeaders(from.Headers, from.Content)
            };

            if(from.Content != null)
            {
                var responseContent = from.Content.ReadAsByteArrayAsync().Result;
                if (responseContent != null)
                {
                    var httpBodyContent = _httpBodyContentMapper.Convert(content: responseContent, headers: to.Headers);

                    to.Body = httpBodyContent.Body;
                }
            }

            return to;
        }

        private Dictionary<string, string> ConvertHeaders(HttpResponseHeaders responseHeaders, HttpContent httpContent)
        {
            if ((responseHeaders == null || !responseHeaders.Any()) &&
                (httpContent == null || (httpContent.Headers == null || !httpContent.Headers.Any())))
            {
                return null;
            }

            var headers = new Dictionary<string, string>();

            if (responseHeaders != null && responseHeaders.Any())
            {
                foreach (var responseHeader in responseHeaders)
                {
                    headers.Add(responseHeader.Key, String.Join(", ", responseHeader.Value.Select(x => x)));
                }
            }

            if (httpContent != null && httpContent.Headers != null && httpContent.Headers.Any())
            {
                foreach (var contentHeader in httpContent.Headers)
                {
                    headers.Add(contentHeader.Key, String.Join(", ", contentHeader.Value.Select(x => x)));
                }
            }

            return headers;
        }
    }
}