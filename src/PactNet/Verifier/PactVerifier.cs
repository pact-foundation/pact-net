using System;
using System.Collections.Generic;
using System.Globalization;
using PactNet.Internal;
using PactNet.Verifier.Messaging;

namespace PactNet.Verifier
{
    /// <summary>
    /// Pact verifier
    /// </summary>
    public class PactVerifier : IPactVerifier
    {
        private readonly PactVerifierConfig config;
        private readonly IDictionary<string, string> verifierArgs;

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
        public PactVerifier(PactVerifierConfig config) : this(new Dictionary<string, string>(), config)
        {
            Guard.NotNull(config, nameof(config));
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        /// <param name="verifierArgs">Pact verifier args</param>
        /// <param name="config">Pact verifier config</param>
        internal PactVerifier(IDictionary<string, string> verifierArgs, PactVerifierConfig config)
        {
            this.config = config;
            this.verifierArgs = verifierArgs;
        }

        /// <summary>
        /// Set the provider details
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <param name="pactUri">URI of the running service</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierProvider ServiceProvider(string providerName, Uri pactUri)
        {
            this.SetProviderHost(providerName, pactUri);

            if (pactUri.AbsolutePath != "/")
            {
                this.verifierArgs.Add("--base-path", pactUri.AbsolutePath);
            }

            return new PactVerifierProvider(this.verifierArgs, this.config);
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
            Guard.NotNull(basePath, nameof(basePath));
            this.SetProviderHost(providerName, pactUri);

            string reconciledPath = pactUri.AbsolutePath != "/" ? new Uri(pactUri, basePath).AbsolutePath : basePath;

            this.verifierArgs.AddOption("--base-path", reconciledPath);

            return new PactVerifierMessagingProvider(this.verifierArgs, this.config);
        }

        /// <summary>
        /// Sets the HTTP request timeout for requests to the target API and for state change requests.
        /// </summary>
        /// <param name="requestTimeout">The timespan represents the request timeout</param>
        /// <returns>The modified PactVerifier instance</returns>
        public IPactVerifier WithRequestTimeout(TimeSpan requestTimeout)
        {
            this.verifierArgs.Add(
                "--request-timeout",
                requestTimeout.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return this;
        }

        private void SetProviderHost(string providerName, Uri pactUri)
        {
            Guard.NotNull(pactUri, nameof(pactUri));

            this.verifierArgs.AddOption("--provider-name", providerName);
            this.verifierArgs.AddOption("--hostname", pactUri.Host);
            this.verifierArgs.AddOption("--port", pactUri.Port.ToString());
        }
    }
}
