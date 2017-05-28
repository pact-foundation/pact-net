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
        public HttpBodyContent Convert(dynamic body, IDictionary<string, object> headers)
        {
            return body == null
                ? null
                : new HttpBodyContent(body, this.ParseContentTypeHeader(headers));
        }

        public HttpBodyContent Convert(byte[] content, IDictionary<string, object> headers)
        {
            return content == null
                ? null
                : new HttpBodyContent(content, this.ParseContentTypeHeader(headers));
        }

        private MediaTypeHeaderValue ParseContentTypeHeader(IDictionary<string, object> headers)
        {
            string contentType = headers?
                .Where(hdr => hdr.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                .Select(hdr => hdr.Value.ToString())
                .FirstOrDefault();

            MediaTypeHeaderValue contentTypeHeader = (contentType == null)
                ? new MediaTypeHeaderValue("text/plain")
                : MediaTypeHeaderValue.Parse(contentType);

            contentTypeHeader.CharSet = contentTypeHeader.CharSet ?? Encoding.UTF8.WebName;

            return contentTypeHeader;
        }
    }
}