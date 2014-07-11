using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public class HttpContentMapper : IHttpContentMapper
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public HttpContent Convert(dynamic from, Encoding encoding, string contentType)
        {
            if (from == null)
            {
                return null;
            }

            StringContent to;
            var jsonRequestBody = JsonConvert.SerializeObject(from, JsonSettings);

            if (encoding != null && !String.IsNullOrEmpty(contentType))
            {
                to = new StringContent(jsonRequestBody, encoding, contentType);
            }
            else if (encoding != null && String.IsNullOrEmpty(contentType))
            {
                to = new StringContent(jsonRequestBody, encoding);
            }
            else if ((encoding == null && !String.IsNullOrEmpty(contentType)))
            {
                to = new StringContent(jsonRequestBody, Encoding.UTF8, contentType);
            }
            else
            {
                to = new StringContent(jsonRequestBody);
            }

            return to;
        }
    }
}