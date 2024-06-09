using System;
using System.IO;
using System.Text.Json;
using PactNet.Internal;
using PactNet.Verifier.Messaging;

namespace PactNet.Verifier
{
    /// <summary>
    /// Pact verifier
    /// </summary>
    public class PactVerifier : IPactVerifier, IDisposable
    {
        private const string VerifierNotInitialised = $"You must add at least one verifier transport by calling {nameof(WithHttpEndpoint)} and/or {nameof(WithMessages)}";

        private readonly string providerName;
        private readonly PactVerifierConfig config;
        private readonly IVerifierProvider provider;
        private readonly IMessagingProvider messagingProvider;

        private bool providerInitialised;
        private bool httpTransportAdded;
        private bool messageTransportAdded;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        /// <param name="providerName">Provider name</param>
        public PactVerifier(string providerName) : this(providerName, new PactVerifierConfig())
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        /// <param name="providerName">Provider name</param>
        /// <param name="config">Pact verifier config</param>
        public PactVerifier(string providerName, PactVerifierConfig config) : this(providerName, config, new InteropVerifierProvider(config), new MessagingProvider(config, new MessageScenarios()))
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        /// <param name="providerName">Provider name</param>
        /// <param name="config">Pact verifier config</param>
        /// <param name="provider">Verifier provider</param>
        /// <param name="messagingProvider">Messaging provider</param>
        internal PactVerifier(string providerName, PactVerifierConfig config, IVerifierProvider provider, IMessagingProvider messagingProvider)
        {
            Guard.NotNull(providerName, nameof(providerName));

            this.providerName = providerName;
            this.config = config;
            this.provider = provider;
            this.messagingProvider = messagingProvider;
        }

        /// <summary>
        /// Add a HTTP endpoint for verifying pacts containing synchronous HTTP interactions
        /// </summary>
        /// <param name="pactUri">URI of the running service</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier WithHttpEndpoint(Uri pactUri)
        {
            Guard.NotNull(pactUri, nameof(pactUri));

            if (this.httpTransportAdded)
            {
                throw new InvalidOperationException("Only one HTTP endpoint can be added");
            }

            if (!this.providerInitialised)
            {
                this.provider.Initialise();
                this.provider.SetProviderInfo(this.providerName, pactUri.Scheme, pactUri.Host, (ushort)pactUri.Port, pactUri.AbsolutePath);
                this.providerInitialised = true;
            }
            else
            {
                this.provider.AddTransport("http", (ushort)pactUri.Port, pactUri.AbsolutePath, pactUri.Scheme);
            }

            this.httpTransportAdded = true;
            return this;
        }

        /// <summary>
        /// Define messages for verifying pacts containing asynchronous message interactions
        /// </summary>
        /// <param name="configure">Configure message scenarios</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier WithMessages(Action<IMessageScenarios> configure) => WithMessages(configure, new JsonSerializerOptions());

        /// <summary>
        /// Define messages for verifying pacts containing asynchronous message interactions
        /// </summary>
        /// <param name="configure">Configure message scenarios</param>
        /// <param name="settings">Settings for serialising messages</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier WithMessages(Action<IMessageScenarios> configure, JsonSerializerOptions settings)
        {
            Guard.NotNull(settings, nameof(settings));

            if (this.messageTransportAdded)
            {
                throw new InvalidOperationException("Only one messaging endpoint can be added");
            }

            // start an in-proc server which creates the messaging responses
            Uri uri = this.messagingProvider.Start(settings);

            if (!this.providerInitialised)
            {
                this.provider.Initialise();
                this.provider.SetProviderInfo(this.providerName, "message", uri.Host, (ushort)uri.Port, uri.AbsolutePath);
                this.providerInitialised = true;
            }
            else
            {
                this.provider.AddTransport("message", (ushort)uri.Port, uri.AbsolutePath, null);
            }

            configure(this.messagingProvider.Scenarios);

            this.messageTransportAdded = true;
            return this;
        }

        /// <summary>
        /// Verify a pact file directly
        /// </summary>
        /// <param name="pactFile">Pact file path</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithFileSource(FileInfo pactFile)
        {
            Guard.NotNull(pactFile, nameof(pactFile));
            Guard.That(this.providerInitialised, VerifierNotInitialised);

            this.provider.AddFileSource(pactFile);

            return new PactVerifierSource(this.provider, this.config);
        }

        /// <summary>
        /// Verify all pacts in the given directory which match the given consumers (or all pact files if no consumers are specified)
        /// </summary>
        /// <param name="directory">Directory containing the pact files</param>
        /// <param name="consumers">(Optional) Consumer names to filter on</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithDirectorySource(DirectoryInfo directory, params string[] consumers)
        {
            Guard.NotNull(directory, nameof(directory));
            Guard.NotNull(consumers, nameof(consumers));
            Guard.That(this.providerInitialised, VerifierNotInitialised);

            this.provider.AddDirectorySource(directory);
            this.provider.SetConsumerFilters(consumers);

            return new PactVerifierSource(this.provider, this.config);
        }

        /// <summary>
        /// Verify a pact from a URI
        /// </summary>
        /// <param name="pactUri">Pact file URI</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithUriSource(Uri pactUri)
            => this.WithUriSource(pactUri, _ => { });

        /// <summary>
        /// Verify a pact from a URI
        /// </summary>
        /// <param name="pactUri">Pact file URI</param>
        /// <param name="configure">Configure URI options</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithUriSource(Uri pactUri, Action<IPactUriOptions> configure)
        {
            Guard.NotNull(pactUri, nameof(pactUri));
            Guard.That(this.providerInitialised, VerifierNotInitialised);

            var options = new PactUriOptions(this.provider, pactUri);
            configure?.Invoke(options);
            options.Apply();

            return new PactVerifierSource(this.provider, this.config);
        }

        /// <summary>
        /// Use the pact broker to retrieve pact files with default options
        /// </summary>
        /// <param name="brokerBaseUri">Base URI for the broker</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithPactBrokerSource(Uri brokerBaseUri)
            => this.WithPactBrokerSource(brokerBaseUri, _ => { });

        /// <summary>
        /// Use the pact broker to retrieve pact files
        /// </summary>
        /// <param name="brokerBaseUri">Base URI for the broker</param>
        /// <param name="configure">Configure pact broker options</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithPactBrokerSource(Uri brokerBaseUri, Action<IPactBrokerOptions> configure)
        {
            Guard.NotNull(brokerBaseUri, nameof(brokerBaseUri));
            Guard.That(this.providerInitialised, VerifierNotInitialised);

            var options = new PactBrokerOptions(this.provider, brokerBaseUri);
            configure?.Invoke(options);
            options.Apply();

            return new PactVerifierSource(this.provider, this.config);
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
