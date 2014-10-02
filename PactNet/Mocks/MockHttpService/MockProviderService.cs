using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Nancy;

namespace PactNet.Mocks.MockHttpService
{
    public class MockProviderService : IMockProviderService
    {
        private readonly Func<Uri, IHttpHost> _hostFactory;
        private IHttpHost _host;
        private readonly HttpClient _httpClient; 

        private string _providerState;
        private string _description;
        private ProviderServiceRequest _request;
        private ProviderServiceResponse _response;

        public string BaseUri { get; private set; }

        internal MockProviderService(
            Func<Uri, IHttpHost> hostFactory,
            int port,
            bool enableSsl,
            Func<string, HttpClient> httpClientFactory)
        {
            _hostFactory = hostFactory;
            BaseUri = String.Format("{0}://localhost:{1}", enableSsl ? "https" : "http", port);
            _httpClient = httpClientFactory(BaseUri);
        }

        public MockProviderService(int port, bool enableSsl)
            : this(
            baseUri => new NancyHttpHost(baseUri), 
            port,
            enableSsl,
            baseUri => new HttpClient { BaseAddress = new Uri(baseUri) })
        {
        }

        public IMockProviderService Given(string providerState)
        {
            if (String.IsNullOrEmpty(providerState))
            {
                throw new ArgumentException("Please supply a non null or empty providerState");
            }

            _providerState = providerState;

            return this;
        }

        public IMockProviderService UponReceiving(string description)
        {
            if (String.IsNullOrEmpty(description))
            {
                throw new ArgumentException("Please supply a non null or empty description");
            }

            _description = description;

            return this;
        }

        public IMockProviderService With(ProviderServiceRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException("Please supply a non null request");
            }

            _request = request;
            
            return this;
        }

        public void WillRespondWith(ProviderServiceResponse response)
        {
            if (response == null)
            {
                throw new ArgumentException("Please supply a non null response");
            }

            _response = response;

            RegisterInteraction();
        }

        public void VerifyInteractions()
        {
            SendAdminHttpRequest(HttpMethod.Get, Constants.InteractionsVerificationPath);
        }

        private void RegisterInteraction()
        {
            if (String.IsNullOrEmpty(_description))
            {
                throw new InvalidOperationException("description has not been set, please supply using the UponReceiving method.");
            }

            if (_request == null)
            {
                throw new InvalidOperationException("request has not been set, please supply using the With method.");
            }

            if (_response == null)
            {
                throw new InvalidOperationException("response has not been set, please supply using the WillRespondWith method.");
            }

            var interaction = new ProviderServiceInteraction
            {
                ProviderState = _providerState,
                Description = _description,
                Request = _request,
                Response = _response
            };

            SendAdminHttpRequest(HttpMethod.Post, Constants.InteractionsPath, interaction);

            ClearTrasientState();
        }

        public void Start()
        {
            StopRunningHost();
            _host = _hostFactory(new Uri(BaseUri));
            _host.Start();
        }

        public void Stop()
        {
            ClearAllState();
            StopRunningHost();
        }

        public void ClearInteractions()
        {
            if (_host != null)
            {
                SendAdminHttpRequest(HttpMethod.Delete, Constants.InteractionsPath);
            }
        }

        public void SendAdminHttpRequest<T>(HttpMethod method, string path, T requestContent) where T : class
        {
            if (_host == null)
            {
                throw new InvalidOperationException("Unable to perform operation because the mock provider service is not running.");
            }

            var responseContent = String.Empty;

            var request = new HttpRequestMessage(method, path);
            request.Headers.Add(Constants.AdministrativeRequestHeaderKey, "true");

            if (requestContent != null)
            {
                var requestContentJson = JsonConvert.SerializeObject(requestContent, JsonConfig.ApiSerializerSettings);
                request.Content = new StringContent(requestContentJson, Encoding.UTF8, "application/json");
            }

            var response = _httpClient.SendAsync(request, CancellationToken.None).Result;
            var responseStatusCode = response.StatusCode;

            if (response.Content != null)
            {
                responseContent = response.Content.ReadAsStringAsync().Result;
            }

            Dispose(request);
            Dispose(response);

            if (responseStatusCode != HttpStatusCode.OK)
            {
                throw new PactFailureException(responseContent);
            }
        }

        private void SendAdminHttpRequest(HttpMethod method, string path)
        {
            SendAdminHttpRequest<object>(method, path, null);
        }

        private void StopRunningHost()
        {
            if (_host != null)
            {
                _host.Stop();
                _host = null;
            }
        }

        private void ClearAllState()
        {
            ClearTrasientState();
            ClearInteractions();
        }

        private void ClearTrasientState()
        {
            _request = null;
            _response = null;
            _providerState = null;
            _description = null;
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
