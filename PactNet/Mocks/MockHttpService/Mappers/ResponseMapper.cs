using System.Collections.Generic;
using System.Net;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal class ResponseMapper : IResponseMapper
    {
        private readonly IHttpBodyContentMapper _httpBodyContentMapper;

        internal ResponseMapper(IHttpBodyContentMapper httpBodyContentMapper)
        {
            _httpBodyContentMapper = httpBodyContentMapper;
        }

        public ResponseMapper() : this(new HttpBodyContentMapper())
        {
        }

        public ResponseWrapper Convert(ProviderServiceResponse from)
        {
            if (from == null)
            {
                return null;
            }

            var to = new ResponseWrapper
            {
                StatusCode = (HttpStatusCode)from.Status,
                Headers = from.Headers ?? new Dictionary<string, string>()
            };

            if (from.Body != null)
            {
                HttpBodyContent bodyContent = _httpBodyContentMapper.Convert(new DynamicBodyMapRequest { Body = from.Body, Headers = from.Headers });
                to.Headers = new Dictionary<string, string> { { "Content-Type", bodyContent.ContentType.MediaType } };
                to.Contents = bodyContent.ContentBytes;
            }

            return to;
        }
    }
}