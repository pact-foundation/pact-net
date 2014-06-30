using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Owin.Testing;
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

        public void VerifyProvider(TestServer server)
        {
            var client = server.HttpClient;
            foreach (var interaction in Interactions)
            {
                var request = new HttpRequestMessage(_httpVerbMap[interaction.Request.Method], interaction.Request.Path);

                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                /*if (interaction.Request.Headers != null && interaction.Request.Headers.Any())
                {
                    foreach (var header in interaction.Request.Headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }*/

                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                };

                if (interaction.Request.Body != null)
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(interaction.Request.Body, jsonSettings));
                }

                var response = client.SendAsync(request).Result;

                var headers = response.Headers;
                
                var actualResponse = new PactProviderResponse
                {
                    Status = (int)response.StatusCode,
                    Body = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result)
                };

                if (!interaction.Response.Equals(actualResponse))
                {
                    throw new Exception("Response does not match");
                    //TODO: Give more details about this!
                }
            }
        }
    }
}