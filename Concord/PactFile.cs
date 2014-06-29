using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Concord
{
    public class PactFile
    {
        public PactParty Provider { get; set; }
        public PactParty Consumer { get; set; }
        public IEnumerable<PactInteraction> Interactions { get; set; }
        public dynamic Metadata { get; set; }

        private Dictionary<HttpVerb, HttpMethod> _httpVerbMap = new Dictionary<HttpVerb, HttpMethod>
        {
            { HttpVerb.Get, HttpMethod.Get },
            { HttpVerb.Post, HttpMethod.Post },
            { HttpVerb.Put, HttpMethod.Put },
            { HttpVerb.Delete, HttpMethod.Delete },
            { HttpVerb.Head, HttpMethod.Head },
            { HttpVerb.Patch, new HttpMethod("PATCH") }
        };

        public void VerifyProvider()
        {
            var client = new HttpClient { BaseAddress = new Uri("http://localhost:1234/") };

            foreach (var interaction in Interactions)
            {
                var request = new HttpRequestMessage(_httpVerbMap[interaction.Request.Method], interaction.Request.Path);

                if (interaction.Request.Headers != null && interaction.Request.Headers.Any())
                {
                    foreach (var header in interaction.Request.Headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                };

                if (interaction.Request.Body != null)
                {
                    request.Content = JsonConvert.SerializeObject(interaction.Request.Body, jsonSettings);
                }
                
                var response = client.SendAsync(request).Result;

                var statusCode = response.StatusCode;
                var headers = response.Headers;
                var body = response.Content.ReadAsStringAsync().Result;

                //TODO:Verify the response is a per the pact response
            }
        }
    }
}