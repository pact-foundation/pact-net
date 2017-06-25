using System;
using System.Net.Http;
using System.Threading;
using PactNet.Core;

namespace PactNet.Mocks.MockHttpService.Host
{
    internal class RubyHttpHost : IHttpHost
    {
        private readonly IPactCoreHost _coreHost;
        private readonly HttpClient _httpClient;

        internal RubyHttpHost(IPactCoreHost coreHost, HttpClient httpClient)
        {
            _coreHost = coreHost;
            _httpClient = httpClient; //TODO: Use the admin http client once extracted
        }

        public RubyHttpHost(Uri baseUri, string providerName, PactConfig config) : 
            this(new PactCoreHost<MockProviderHostConfig>(
                new MockProviderHostConfig(baseUri.Port, 
                    baseUri.Scheme.ToUpperInvariant().Equals("HTTPS"), 
                    providerName, config)),
                new HttpClient { BaseAddress = baseUri })
        {
        }

        private bool IsMockProviderServiceRunning()
        {
            try
            {
                var aliveRequest = new HttpRequestMessage(HttpMethod.Get, "/");
                aliveRequest.Headers.Add(Constants.AdministrativeRequestHeaderKey, "true");

                var responseMessage = _httpClient.SendAsync(aliveRequest).Result;
                return responseMessage.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void Start()
        {
            _coreHost.Start();

            var aliveChecks = 1;
            while (!IsMockProviderServiceRunning())
            {
                if (aliveChecks >= 20)
                {
                    throw new PactFailureException("Could not start the mock provider service");
                }

                Thread.Sleep(200);
                aliveChecks++;
            }
        }

        public void Stop()
        {
            _coreHost.Stop();
        }
    }
}