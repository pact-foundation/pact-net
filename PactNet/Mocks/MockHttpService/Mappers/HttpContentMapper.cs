using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using PactNet.Configuration.Json;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public class HttpContentMapper : IHttpContentMapper
    {
        public HttpContent Convert(dynamic from, Encoding encoding, string contentType)
        {
            if (from == null)
            {
                return null;
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            if (String.IsNullOrEmpty(contentType))
            {
                contentType = "text/plain";
            }

            var body = !String.IsNullOrEmpty(contentType) &&
                       contentType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase)
                ? JsonConvert.SerializeObject(from, JsonConfig.ApiRequestSerializerSettings)
                : from;

            return new StringContent(body, encoding, contentType);
        }
    }
}