using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using static System.String;

namespace PactNet.Backend.Remote
{
    internal class AdminHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpMethodMapper _httpMethodMapper;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        internal AdminHttpClient(
            Uri baseUri,
            HttpMessageHandler handler,
            JsonSerializerSettings jsonSerializerSettings)
        {
            this._httpClient = new HttpClient(handler) { BaseAddress = baseUri };
            this._httpMethodMapper = new HttpMethodMapper();
            this._jsonSerializerSettings = jsonSerializerSettings ?? JsonConfig.ApiSerializerSettings;
        }

        public AdminHttpClient(Uri baseUri) :
            this(baseUri, new HttpClientHandler(), null)
        {
        }

        public AdminHttpClient(Uri baseUri, JsonSerializerSettings jsonSerializerSettings) :
            this(baseUri, new HttpClientHandler(), jsonSerializerSettings)
        {
        }

        public async Task<string> SendAdminHttpRequest(HttpVerb method, string path, IDictionary<string, string> headers = null)
        {
            return await this.SendAdminHttpRequest<object>(method, path, null, headers: headers);
        }

        public async Task<string> SendAdminHttpRequest<T>(HttpVerb method, string path, T requestContent, IDictionary<string, string> headers = null) where T : class
        {
            var request = new HttpRequestMessage(this._httpMethodMapper.Convert(method), path);
            request.Headers.Add(Constants.AdministrativeRequestHeaderKey, "true");

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (requestContent != null)
            {
                var requestContentJson = JsonConvert.SerializeObject(requestContent, this._jsonSerializerSettings);
                request.Content = new StringContent(requestContentJson, Encoding.UTF8, "application/json");
            }

            var response = await this._httpClient.SendAsync(request, CancellationToken.None);
            var responseStatusCode = response.StatusCode;
            var responseContent = Empty;

            if (response.Content != null)
            {
                responseContent = await response.Content.ReadAsStringAsync();
            }

            this.Dispose(request);
            this.Dispose(response);

            if (responseStatusCode != HttpStatusCode.OK)
            {
                throw new PactFailureException(responseContent);
            }

            return !string.IsNullOrEmpty(responseContent) ? responseContent : Empty;
        }

        private void Dispose(IDisposable disposable)
        {
            disposable?.Dispose();
        }
    }
}