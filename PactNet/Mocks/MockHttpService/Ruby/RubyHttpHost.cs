using System;
using System.Net.Http;
using System.Threading;
using PactNet.Core;

namespace PactNet.Mocks.MockHttpService.Ruby
{
    internal class RubyHttpHost : IHttpHost
    {
        private readonly PactProcessHost<MockProviderConfiguration> _processHost;
        private readonly HttpClient _httpClient;

        public RubyHttpHost(int port, bool enableSsl, string providerName, PactConfig config)
        {
            _processHost = new PactProcessHost<MockProviderConfiguration>(
                new MockProviderConfiguration(port, enableSsl, providerName, config));

            //TODO: Use the admin http client once extracted
            _httpClient = new HttpClient { BaseAddress = new Uri($"{(enableSsl ? "https" : "http")}://localhost:{port}") };
        }

        private bool IsMockProviderServiceRunning()
        {
            var aliveRequest = new HttpRequestMessage(HttpMethod.Get, "/");
            aliveRequest.Headers.Add(Constants.AdministrativeRequestHeaderKey, "true");

            var responseMessage = _httpClient.SendAsync(aliveRequest).Result;
            return responseMessage.IsSuccessStatusCode;
        }

        public void Start()
        {
            _processHost.Start();

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
            _processHost.Stop();
        }
    }
}