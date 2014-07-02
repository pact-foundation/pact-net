using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Concord
{
    public class PactFile
    {
        private IList<PactInteraction> _interactions;

        public PactParty Provider { get; set; }
        public PactParty Consumer { get; set; }

        public IEnumerable<PactInteraction> Interactions
        {
            get { return _interactions; }
            set { _interactions = value.ToList(); }
        }

        public dynamic Metadata { get; set; }

        public PactFile()
        {
            Metadata = new
            {
                PactSpecificationVersion = "1.0.0"
            };
        }

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

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
                var requestHeaders = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

                if (interaction.Request.Headers != null && interaction.Request.Headers.Any())
                {
                    foreach (var header in interaction.Request.Headers)
                    {
                        requestHeaders.Add(header.Key, header.Value);
                    }
                }

                var request = new HttpRequestMessage(_httpVerbMap[interaction.Request.Method], interaction.Request.Path);

                if (interaction.Request.Body != null)
                {
                    //If there is a content-type header add it to the content
                    var jsonContent = JsonConvert.SerializeObject(interaction.Request.Body, _jsonSettings);
                    StringContent content;
                    if (requestHeaders.ContainsKey("Content-Type"))
                    {
                        // TODO: This is extremely icky. Need a better way of dealing with this.
                        var contentType = requestHeaders["Content-Type"].Split(';');
                        content = new StringContent(jsonContent, Encoding.UTF8, contentType[0]);
                    }
                    else
                    {
                        content = new StringContent(jsonContent);
                    }

                    request.Content = content;
                }

                if (interaction.Request.Headers != null && interaction.Request.Headers.Any())
                {
                    foreach (var requestHeader in interaction.Request.Headers)
                    {
                        //Ignore any content headers, as they will be attached above if applicable
                        if (requestHeader.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        request.Headers.Add(requestHeader.Key, requestHeader.Value);
                    }
                }

                var response = client.SendAsync(request).Result;

                var actualResponse = new PactProviderResponse
                {
                    Status = (int)response.StatusCode,
                    Body = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result),
                    Headers = ConvertHeaders(response.Headers, response.Content.Headers)
                };

                PactAssert.Equal(interaction.Response, actualResponse);
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

        public void AddInteraction(PactInteraction interation)
        {
            _interactions = _interactions ?? new List<PactInteraction>();

            if (_interactions.Any(x => x.Description == interation.Description))
            {
                var interactionToReplace = _interactions.Single(x => x.Description == interation.Description);
                _interactions[_interactions.IndexOf(interactionToReplace)] = interation;
            }
            else
            {
                _interactions.Add(interation);
            }
        }
    }
}