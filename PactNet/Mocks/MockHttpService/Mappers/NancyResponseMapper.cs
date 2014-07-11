using System;
using System.Collections.Generic;
using Nancy;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public class NancyResponseMapper : INancyResponseMapper
    {
        private readonly IHttpBodyContentMapper _httpBodyContentMapper;

        [Obsolete("For testing only.")]
        public NancyResponseMapper(IHttpBodyContentMapper httpBodyContentMapper)
        {
            _httpBodyContentMapper = httpBodyContentMapper;
        }
        public NancyResponseMapper() : this(
            new HttpBodyContentMapper())
        {
        }

        public Response Convert(PactProviderServiceResponse from)
        {
            if (from == null)
            {
                return null;
            }

            var to = new Response
            {
                StatusCode = (HttpStatusCode) from.Status,
                Headers = from.Headers
            };

            if (from.Body != null)
            {
                HttpBodyContent bodyContent = _httpBodyContentMapper.Convert(from.Body, to.Headers);

                to.Contents = s =>
                {
                    byte[] bytes = bodyContent.ContentBytes;
                    s.Write(bytes, 0, bytes.Length);
                    s.Flush();
                };
            }
            else
            {
                to.Headers = to.Headers ?? new Dictionary<string, string>();

                if (!to.Headers.ContainsKey("Content-Length"))
                {
                    to.Headers.Add("Content-Length", "0");
                }
                else
                {
                    to.Headers["Content-Length"] = "0";
                }
            }

            return to;
        }
    }
}