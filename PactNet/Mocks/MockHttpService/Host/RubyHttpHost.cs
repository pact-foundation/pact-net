using System;
using System.Threading;
using PactNet.Core;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Host
{
    internal class RubyHttpHost : IHttpHost
    {
        private readonly IPactCoreHost _coreHost;
        private readonly AdminHttpClient _adminHttpClient;

        internal RubyHttpHost(
            IPactCoreHost coreHost, 
            AdminHttpClient adminHttpClient)
        {
            _coreHost = coreHost;
            _adminHttpClient = adminHttpClient;
        }

        public RubyHttpHost(Uri baseUri, string providerName, PactConfig config) : 
            this(new PactCoreHost<MockProviderHostConfig>(
                new MockProviderHostConfig(baseUri.Port, 
                    baseUri.Scheme.ToUpperInvariant().Equals("HTTPS"), 
                    providerName, config)),
                new AdminHttpClient(baseUri))
        {
        }

        private bool IsMockProviderServiceRunning()
        {
            try
            {
                _adminHttpClient.SendAdminHttpRequest(HttpVerb.Get, "/");
                return true;
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