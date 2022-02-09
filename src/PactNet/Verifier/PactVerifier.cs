using System;
using PactNet.Internal;
using PactNet.Verifier.Messaging;

namespace PactNet.Verifier
{
    /// <summary>
    /// Pact verifier
    /// </summary>
    public class PactVerifier : IPactVerifier, IDisposable
    {
        private readonly PactVerifierConfig config;
        private readonly IVerifierProvider provider;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        public PactVerifier() : this(new PactVerifierConfig())
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        /// <param name="config">Pact verifier config</param>
        public PactVerifier(PactVerifierConfig config) : this(new InteropVerifierProvider(config), config)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        /// <param name="provider">Verifier provider</param>
        /// <param name="config">Pact verifier config</param>
        internal PactVerifier(IVerifierProvider provider, PactVerifierConfig config)
        {
            Guard.NotNull(config, nameof(config));

            this.config = config;
            this.provider = provider;
        }

        /// <summary>
        /// Set the provider details
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <param name="pactUri">URI of the running service</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierProvider ServiceProvider(string providerName, Uri pactUri)
        {
            Guard.NotNullOrEmpty(providerName, nameof(providerName));
            Guard.NotNull(pactUri, nameof(pactUri));

            this.InitialiseProvider(providerName, pactUri);

            return new PactVerifierProvider(this.provider, this.config);
        }

        /// <summary>
        /// Set the provider details of a messaging provider
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <param name="pactUri">URI of the running service</param>
        /// <param name="basePath">Path of the messaging provider endpoint</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierMessagingProvider MessagingProvider(string providerName, Uri pactUri, string basePath)
        {
            Guard.NotNullOrEmpty(providerName, nameof(providerName));
            Guard.NotNull(pactUri, nameof(pactUri));
            Guard.NotNull(basePath, nameof(basePath));

            var uriWithBasePath = new Uri(pactUri, basePath);
            this.InitialiseProvider(providerName, uriWithBasePath);

            return new PactVerifierMessagingProvider(this.provider, this.config);
        }

        /// <summary>
        /// Initialise the verifier provider
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="pactUri"></param>
        private void InitialiseProvider(string providerName, Uri pactUri)
        {
            Guard.NotNull(pactUri, nameof(pactUri));

            this.provider.Initialise();
            this.provider.SetProviderInfo(providerName,
                                          pactUri.Scheme,
                                          pactUri.Host,
                                          (ushort)pactUri.Port,
                                          pactUri.AbsolutePath);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.provider?.Dispose();
        }
    }
}
