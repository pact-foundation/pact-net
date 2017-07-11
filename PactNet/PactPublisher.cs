using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Extensions;
using PactNet.Models;

namespace PactNet
{
    public class PactPublisher
    {
        private readonly HttpClient _httpClient;
        private readonly PactUriOptions _brokerUriOptions;

        internal PactPublisher(
            Uri baseUri,
            PactUriOptions brokerUriOptions,
            HttpMessageHandler handler)
        {
            _httpClient = new HttpClient(handler) { BaseAddress = baseUri };
            _brokerUriOptions = brokerUriOptions;
        }

        public PactPublisher(Uri brokerBaseUri, PactUriOptions brokerUriOptions = null) : 
            this(brokerBaseUri, brokerUriOptions, new HttpClientHandler())
        {
        }

        public void PublishToBroker(string pactFileUri, string consumerVersion, IEnumerable<string> tags = null)
        {
            if (String.IsNullOrEmpty(pactFileUri))
            {
                throw new ArgumentNullException("pactFileUri is null or empty");
            }

            if (String.IsNullOrEmpty(consumerVersion))
            {
                throw new ArgumentNullException("consumerVersion is null or empty");
            }

            var pactFileText = File.ReadAllText(pactFileUri);
            var pactFile = JsonConvert.DeserializeObject<PactFile>(pactFileText);

            var request = new HttpRequestMessage(HttpMethod.Put, $"/pacts/provider/{pactFile.Provider.Name}/consumer/{pactFile.Consumer.Name}/version/{consumerVersion}");

            if (_brokerUriOptions != null)
            {
                request.Headers.Add("Authorization", $"{_brokerUriOptions.AuthorizationScheme} {_brokerUriOptions.AuthorizationValue}");
            }

            var requestContentJson = JsonConvert.SerializeObject(request, JsonConfig.ApiSerializerSettings);
            request.Content = new StringContent(requestContentJson, Encoding.UTF8, "application/json");

            var response = _httpClient.SendAsync(request, CancellationToken.None).RunSync();
            var responseStatusCode = response.StatusCode;
            var responseContent = String.Empty;

            Dispose(request);
            Dispose(response);

            if (response.Content != null)
            {
                responseContent = response.Content.ReadAsStringAsync().RunSync();
            }

            if (responseStatusCode != HttpStatusCode.OK)
            {
                throw new PactFailureException($"Failed to publish Pact to the broker: {responseContent}");
            }

            if (tags != null && tags.Any())
            {
                foreach (var tag in tags)
                {
                    var tagRequest = new HttpRequestMessage(HttpMethod.Put, $"/pacticipants/{pactFile.Consumer.Name}/versions/{consumerVersion}/tags/{tag}");

                    if (_brokerUriOptions != null)
                    {
                        tagRequest.Headers.Add("Authorization", $"{_brokerUriOptions.AuthorizationScheme} {_brokerUriOptions.AuthorizationValue}");
                    }

                    var tagResponse = _httpClient.SendAsync(tagRequest, CancellationToken.None).RunSync();
                    var tagResponseStatusCode = tagResponse.StatusCode;
                    var tagResponseContent = String.Empty;

                    if (tagResponse.Content != null)
                    {
                        tagResponseContent = tagResponse.Content.ReadAsStringAsync().RunSync();
                    }

                    Dispose(tagRequest);
                    Dispose(tagResponse);

                    if (tagResponseStatusCode != HttpStatusCode.OK)
                    {
                        throw new PactFailureException($"Failed to add Pact tag '{tag}' to the broker: {tagResponseContent}");
                    }
                }
            }
        }

        private void Dispose(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}