using System.Collections.Generic;
using Nancy;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal class NancyResponseMapper : INancyResponseMapper
    {
        private readonly IHttpBodyContentMapper _httpBodyContentMapper;

        internal NancyResponseMapper(IHttpBodyContentMapper httpBodyContentMapper)
        {
            _httpBodyContentMapper = httpBodyContentMapper;
        }

        public NancyResponseMapper() : this(new HttpBodyContentMapper())
        {
        }

        public Response Convert(ProviderServiceResponse from)
        {
            if (from == null)
            {
                return null;
            }

            var to = new Response
            {
                StatusCode = (HttpStatusCode)from.Status,
                Headers = from.Headers ?? new Dictionary<string, string>()
            };

            if (from.Body != null)
            {
                HttpBodyContent bodyContent = _httpBodyContentMapper.Convert(body: from.Body, headers: from.Headers);
                to.ContentType = bodyContent.ContentType.MediaType;
                to.Contents = s =>
                {
                    byte[] bytes = bodyContent.ContentBytes;
                    s.Write(bytes, 0, bytes.Length);
                    s.Flush();
                };
            }

            return to;
        }
    }
}