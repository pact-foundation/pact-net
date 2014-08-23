using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Nancy.Hosting.Self;
using PactNet.Mocks.MockHttpService.Configuration;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Nancy;
using PactNet.Models;

namespace PactNet.Mocks.MockHttpService
{
    public class MockProviderService : IMockProviderService
    {
        private readonly Func<Uri, IMockContextService, NancyHost> _nancyHostFactory;
        private NancyHost _host;
        private readonly Func<string, HttpClient> _httpClientFactory; 

        private string _providerState;
        private string _description;
        private ProviderServiceRequest _request;
        private ProviderServiceResponse _response;
        private IList<ProviderServiceInteraction> _testScopedInteractions;

        private IList<ProviderServiceInteraction> _interactions;
        public IEnumerable<Interaction> Interactions
        {
            get { return _interactions; }
        }

        public string BaseUri { get; private set; }

        [Obsolete("For testing only.")]
        public MockProviderService(
            Func<Uri, IMockContextService, NancyHost> nancyHostFactory, 
            int port,
            Func<string, HttpClient> httpClientFactory)
        {
            _nancyHostFactory = nancyHostFactory;
            BaseUri = String.Format("http://localhost:{0}", port);
            _httpClientFactory = httpClientFactory;
        }

        public MockProviderService(int port)
            : this(
            (baseUri, mockContextService) => new NancyHost(new MockProviderNancyBootstrapper(mockContextService), NancyConfig.HostConfiguration, baseUri), 
            port,
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
            if (_testScopedInteractions == null)
            {
                return;
            }

            if (_host != null)
            {
                var client = _httpClientFactory(BaseUri);
                var request = new HttpRequestMessage(HttpMethod.Get, "/interactions/verification");
                request.Headers.Add(Constants.AdministrativeRequestHeaderKey, "");
                var response = client.SendAsync(request, CancellationToken.None).Result;
                response.EnsureSuccessStatusCode();
            }
            else
            {
                throw new InvalidOperationException("Unable to verify interactions because the mock provider service is not running.");
            }

            var unUsedInteractions = _testScopedInteractions.Where(interaction => interaction.UsageCount < 1).ToList();
            var unUsedInteractionsErrorMessage = "";

            if (unUsedInteractions.Any())
            {
                unUsedInteractionsErrorMessage = "The following interactions were not used by the test: " + String.Join(", ",
                    unUsedInteractions.Select(interaction => String.Format("{0}", interaction.Summary()))) + ". ";
            }

            var multiUsedInteractions = _testScopedInteractions.Where(interaction => interaction.UsageCount > 1).ToList();
            var multiUsedInteractionsErrorMessage = "";

            if (multiUsedInteractions.Any())
            {
                multiUsedInteractionsErrorMessage = "The following interactions were called more than once by the test: " + String.Join(", ",
                    multiUsedInteractions.Select(interaction => String.Format("{0} [{1} time/s]", interaction.Summary(), interaction.UsageCount))) + ". ";
            }

            if (unUsedInteractions.Any() || multiUsedInteractions.Any())
            {
                throw new InvalidOperationException(unUsedInteractionsErrorMessage + multiUsedInteractionsErrorMessage);
            }
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

            _testScopedInteractions = _testScopedInteractions ?? new List<ProviderServiceInteraction>();
            _interactions = _interactions ?? new List<ProviderServiceInteraction>();

            if (_testScopedInteractions.Any(x => x.Description == interaction.Description &&
                x.ProviderState == interaction.ProviderState))
            {
                throw new InvalidOperationException(String.Format("An interaction already exists with the description \"{0}\" and provider state \"{1}\". Please supply a different description or provider state.", interaction.Description, interaction.ProviderState));
            }

            if (!_interactions.Any(x => x.Description == interaction.Description &&
                x.ProviderState == interaction.ProviderState))
            {
                _interactions.Add(interaction);
            }

            _testScopedInteractions.Add(interaction);

            ClearTrasientState();
        }

        public void Start()
        {
            _host = _nancyHostFactory(new Uri(BaseUri), new MockContextService(GetMockInteractions));
            _host.Start();
        }

        public void Stop()
        {
            ClearAllState();
            if (_host != null)
            {
                _host.Stop();
                _host.Dispose();
            }
        }

        public void ClearInteractions()
        {
            _testScopedInteractions = null;

            if (_host != null)
            {
                var client = _httpClientFactory(BaseUri);
                var request = new HttpRequestMessage(HttpMethod.Delete, "/interactions");
                request.Headers.Add(Constants.AdministrativeRequestHeaderKey, "");
                var response = client.SendAsync(request, CancellationToken.None).Result;
                response.EnsureSuccessStatusCode();
            }
            else
            {
                //Maybe this is ok??
                throw new InvalidOperationException("Unable to clear interactions because the mock provider service is not running.");
            }
        }

        private void ClearAllState()
        {
            ClearTrasientState();
            ClearInteractions();
            _interactions = null;
        }

        private void ClearTrasientState()
        {
            _request = null;
            _response = null;
            _providerState = null;
            _description = null;
        }

        private IEnumerable<ProviderServiceInteraction> GetMockInteractions()
        {
            if (_testScopedInteractions == null || !_testScopedInteractions.Any())
            {
                return null;
            }

            return _testScopedInteractions;
        }
    }
}
