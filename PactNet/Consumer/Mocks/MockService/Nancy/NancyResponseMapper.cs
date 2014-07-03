using System.Collections.Generic;
using System.Text;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PactNet.Consumer.Mocks.MockService.Nancy
{
    public class NancyResponseMapper
    {
        public Response Convert(PactProviderResponse from)
        {
            if (from == null)
                return null;

            var to = new Response
            {
                StatusCode = (HttpStatusCode) from.Status,
                Headers = from.Headers
            };
            

            if (from.Body != null)
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                string jsonBody = JsonConvert.SerializeObject(from.Body, jsonSettings);
                to.Contents = s =>
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(jsonBody);
                    s.Write(bytes, 0, bytes.Length);
                    s.Flush();
                };
            }
            else
            {
                to.Headers = to.Headers ?? new Dictionary<string, string>();
                to.Headers.Add("Content-Length", "0");
            }

            return to;
        }
    }
}