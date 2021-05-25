using System;
using PactNet.Backend.Native;
using PactNet.Backend.Remote;
using PactNet.Models;

namespace PactNet
{
    /// <summary>
    /// Pact builder
    /// </summary>
    public class PactBuilder : IPactBuilder
    {
        private readonly PactConfig config;

        private string consumer;
        private string provider;
        private IMockServer service;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactBuilder"/> class.
        /// </summary>
        public PactBuilder() : this(new PactConfig())
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactBuilder"/> class.
        /// </summary>
        /// <param name="config">Pact config</param>
        public PactBuilder(PactConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Establish the consumer name
        /// </summary>
        /// <param name="consumerName">Name of the consumer</param>
        /// <returns>Same pact builder</returns>
        public IPactBuilder ServiceConsumer(string consumerName)
        {
            if (string.IsNullOrEmpty(consumerName))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            this.consumer = consumerName;

            return this;
        }

        /// <summary>
        /// Establish the provider name
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <returns>Same pact builder</returns>
        public IPactBuilder HasPactWith(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                throw new ArgumentException("Please supply a non null or empty providerName");
            }

            this.provider = providerName;

            return this;
        }

        /// <summary>
        /// Start a mock server running locally in-process to build up the pact
        /// </summary>
        /// <param name="port">Port for the mock server. If null, one will be assigned automatically</param>
        /// <param name="host">Host for the mock server</param>
        /// <returns>Interaction builder</returns>
        public IInteractionBuilder UsingNativeBackend(int? port = null, IPAddress host = IPAddress.Loopback)
        {
            if (string.IsNullOrWhiteSpace(this.consumer) || string.IsNullOrWhiteSpace(this.provider))
            {
                throw new InvalidOperationException("Make sure you have set both consumer and provider name before starting the service");
            }

            var rustService = new NativeMockServer(this.consumer,
                                                 this.provider,
                                                 this.config,
                                                 port,
                                                 host);
            this.service = rustService;

            IInteractionBuilder interactionBuilder = rustService.CreatePact();
            return interactionBuilder;
        }

        /// <summary>
        /// Use an existing remote server running at the given URI
        /// </summary>
        /// <param name="uri">Remote server URI</param>
        /// <returns>Interaction builder</returns>
        public IInteractionBuilder UsingRemoteBackend(Uri uri)
        {
            if (string.IsNullOrWhiteSpace(this.consumer) || string.IsNullOrWhiteSpace(this.provider))
            {
                throw new InvalidOperationException("Make sure you have set both consumer and provider name before starting the service");
            }

            var remoteService = new RemoteMockServer(this.consumer, this.provider, this.config);
            this.service = remoteService;

            IInteractionBuilder interactionBuilder = remoteService.CreatePact();
            return interactionBuilder;
        }

        /// <summary>
        /// After all interactions are complete, write the pact file
        /// </summary>
        public void Build()
        {
            if (this.service == null)
            {
                throw new InvalidOperationException("Unable to save the pact because the server has not been started");
            }

            this.service.WritePactFile();

            if (this.service is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}