using System;
using Nancy.Hosting.Self;
using PactNet.Mocks.MockHttpService.Configuration;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    //TODO: Test this
    public class NancyHttpHost : IHttpHost
    {
        private readonly Uri _baseUri;
        private readonly IMockContextService _mockContextService;
        private NancyHost _host;

        public NancyHttpHost(
            Uri baseUri, 
            IMockContextService mockContextService)
        {
            _baseUri = baseUri;
            _mockContextService = mockContextService;
        }

        public void Start()
        {
            Stop();
            _host = new NancyHost(new MockProviderNancyBootstrapper(_mockContextService), NancyConfig.HostConfiguration, _baseUri);
            _host.Start();
        }

        public void Stop()
        {
            if (_host != null)
            {
                _host.Stop();
                _host.Dispose();
                _host = null;
            }
        }
    }
}