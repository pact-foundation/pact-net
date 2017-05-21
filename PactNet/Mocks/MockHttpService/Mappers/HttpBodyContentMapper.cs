using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal class HttpBodyContentMapper : IHttpBodyContentMapper
    {
        public HttpBodyContent Convert(DynamicBodyMapRequest request)
        {
            return request?.Body == null
                ? null 
                : new HttpBodyContent(new DynamicBody { Body = request.Body, ContentType = ParseContentTypeHeader(request.Headers) });
        }

        public HttpBodyContent Convert(BinaryContentMapRequest request)
        {
            return request?.Content == null
                ? null
                : new HttpBodyContent(new BinaryContent { Content = request.Content, ContentType = ParseContentTypeHeader(request.Headers) });
        }

        private MediaTypeHeaderValue ParseContentTypeHeader(IDictionary<string, string> headers)
        {
            string contentType = headers?
                .Where(hdr => hdr.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                .Select(hdr => hdr.Value)
                .FirstOrDefault();

            MediaTypeHeaderValue contentTypeHeader = (contentType == null)
                ? new MediaTypeHeaderValue("text/plain")
                : MediaTypeHeaderValue.Parse(contentType);

            contentTypeHeader.CharSet = contentTypeHeader.CharSet ?? Encoding.UTF8.WebName;

            return contentTypeHeader;
        }
    }
}