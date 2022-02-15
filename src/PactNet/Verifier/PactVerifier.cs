using System;
using Newtonsoft.Json;
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
        private readonly IMessagingProvider messagingProvider;

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
        public PactVerifier(PactVerifierConfig config) : this(new InteropVerifierProvider(config), new MessagingProvider(config, new MessageScenarios()), config)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        /// <param name="provider">Verifier provider</param>
        /// <param name="messagingProvider">Messaging provider</param>
        /// <param name="config">Pact verifier config</param>
        internal PactVerifier(IVerifierProvider provider, IMessagingProvider messagingProvider, PactVerifierConfig config)
        {
            Guard.NotNull(config, nameof(config));

            this.config = config;
            this.provider = provider;
            this.messagingProvider = messagingProvider;
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
        /// <returns>Fluent builder</returns>
        public IPactVerifierMessagingProvider MessagingProvider(string providerName)
            => MessagingProvider(providerName, new JsonSerializerSettings());

        /// <summary>
        /// Set the provider details of a messaging provider
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <param name="settings">Default JSON serialisation settings</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierMessagingProvider MessagingProvider(string providerName, JsonSerializerSettings settings)
        {
            Guard.NotNullOrEmpty(providerName, nameof(providerName));
            Guard.NotNull(settings, nameof(settings));

            // start an in-proc server which creates the messaging responses
            Uri uri = this.messagingProvider.Start(settings);

            this.InitialiseProvider(providerName, uri);

            return new PactVerifierMessagingProvider(this.provider, this.messagingProvider.Scenarios, this.config);
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
            this.messagingProvider?.Dispose();
            this.provider?.Dispose();
        }
    }
}
