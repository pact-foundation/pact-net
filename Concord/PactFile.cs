using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public void VerifyProvider(HttpClient client)
        {
            foreach (var interaction in Interactions)
            {
                var request = new HttpRequestMessage(_httpVerbMap[interaction.Request.Method], interaction.Request.Path);

                if (interaction.Request.Headers != null && interaction.Request.Headers.Any())
                {
                    foreach (var header in interaction.Request.Headers)
                    {
                        if (header.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                        {
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(header.Value));
                        }
                        else
                        {
                            request.Headers.Add(header.Key, header.Value);
                        }
                    }
                }

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

                
                var actualResponse = new PactProviderResponse
                {
                    Status = (int)response.StatusCode,
                    Body = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result),
                    Headers = ConvertHeaders(response.Headers, response.Content.Headers)
                };

                if (!interaction.Response.Equals(actualResponse))
                {
                    throw new Exception("Response does not match");
                    //TODO: Give more details about this!
                }
            }
        }

        private Dictionary<string, string> ConvertHeaders(HttpResponseHeaders responseHeaders, HttpContentHeaders contentHeaders)
        {
            if ((responseHeaders == null || !responseHeaders.Any()) &&
                (contentHeaders == null || !contentHeaders.Any()))
            {
                return null;
            }

            var headers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var responseHeader in responseHeaders)
            {
                headers.Add(responseHeader.Key, responseHeader.Value.First());
            }

            foreach (var contentHeader in contentHeaders)
            {
                headers.Add(contentHeader.Key, contentHeader.Value.First());
            }

            return headers;
        }
    }
}