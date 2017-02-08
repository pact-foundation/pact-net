using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using PactNet.Models.Messaging;

namespace PactNet
{
    public class PactBrokerClient : IDisposable
    {
        private HttpClient _httpClient;
        private PactUriOptions _options;

        public PactBrokerClient(PactUriOptions options, HttpClient client)
        {
            _options = options;
            _httpClient = client;
        }

        public PactBrokerClient(PactUriOptions options)
            :this(options, new HttpClient())
        {
        }

        public PactBrokerClient(HttpClient client)
            :this(null, client)
        {
        }

        public PactBrokerClient()
            :this(null, new HttpClient())
        {
        }

        public PactUriOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        public IEnumerable<string> GetPactsByProvider(Uri baseUri, string provider)
        {
            var uri = new Uri(baseUri, string.Format("pacts/provider/{0}/latest", provider));

            var pactFiles = new List<string>();
            using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                request.Headers.Add("Accept", "application/hal+json");

                if (_options != null)
                {
                    request.Headers.Add("Authorization", String.Format("{0} {1}", _options.AuthorizationScheme, _options.AuthorizationValue));
                }

                using (var response = _httpClient.SendAsync(request).Result)
                {
                    response.EnsureSuccessStatusCode();
                    var providerPacts = response.Content.ReadAsStringAsync().Result;

                    var content = JObject.Parse(providerPacts);

                    foreach (var pactUri in content.SelectTokens("$._links.pacts[*].href"))
                        pactFiles.Add(GetPactFile(new Uri(pactUri.Value<string>())));
                }
            }

            return pactFiles;
        }

        public string GetPactFile(Uri pactUri)
        {
            var pactFile = string.Empty;

            using (var request = new HttpRequestMessage(HttpMethod.Get, pactUri))
            {
                request.Headers.Add("Accept", "application/json");

                if (_options != null)
                {
                    request.Headers.Add("Authorization", String.Format("{0} {1}", _options.AuthorizationScheme, _options.AuthorizationValue));
                }

                using (var response = _httpClient.SendAsync(request).Result)
                {
                    response.EnsureSuccessStatusCode();
                    pactFile = response.Content.ReadAsStringAsync().Result;
                }
            }

            return pactFile;
        }

        public string GetPactFile(Uri baseUri, string provider, string consumer, string version)
        {
            return this.GetPactFile(new Uri(baseUri,
                    string.Format("pacts/provider/{0}/consumer/{1}/version/{2}", provider, consumer, version)));
        }

        public string GetPactFile(Uri baseUri, string provider, string consumer)
        {
            return this.GetPactFile(baseUri, provider, consumer, "latest");
        }

        public void PutPactFile(Uri baseUri, string pactJson, string provider, string consumer, string version)
        {
            var pactUri = new Uri(baseUri,
                string.Format("pacts/provider/{0}/consumer/{1}/version/{2}", provider, consumer, version));

            using (var request = new HttpRequestMessage(HttpMethod.Put, pactUri))
            {
                request.Headers.Add("Content-Type", "application/json");

                if (_options != null)
                {
                    request.Headers.Add("Authorization", String.Format("{0} {1}", _options.AuthorizationScheme, _options.AuthorizationValue));
                }

                request.Content = new StringContent(pactJson);

                using (var response = _httpClient.SendAsync(request).Result)
                {
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        public void Dispose()
        {
            if (_httpClient != null)
                _httpClient.Dispose();

            _httpClient = null;
        }
    }
}
