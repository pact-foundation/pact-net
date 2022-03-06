using System;
using PactNet.Exceptions;
using PactNet.Internal;

namespace PactNet.Verifier
{
    /// <summary>
    /// Native pact verifier source type state
    /// </summary>
    internal class PactVerifierSource : IPactVerifierSource
    {
        private readonly IVerifierProvider provider;
        private readonly PactVerifierConfig config;

        private TimeSpan timeout = TimeSpan.FromSeconds(5);
        private bool disableSslVerification = false;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifierSource"/> class.
        /// </summary>
        /// <param name="provider">Pact verifier provider</param>
        /// <param name="config">Verifier config</param>
        internal PactVerifierSource(IVerifierProvider provider, PactVerifierConfig config)
        {
            this.provider = provider;
            this.config = config;
        }

        /// <summary>
        /// Set up the provider state setup URI so the service can configure states
        /// </summary>
        /// <param name="providerStateUri">Provider state URI</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithProviderStateUrl(Uri providerStateUri)
        {
            Guard.NotNull(providerStateUri, nameof(providerStateUri));

            // TODO: Support teardowns and disabling provider state bodies
            this.provider.SetProviderState(providerStateUri, false, true);

            return this;
        }

        /// <summary>
        /// Filter the interactions to only those matching the given description and/or provider state
        /// </summary>
        /// <param name="description">Interaction description. All interactions are verified if this is null</param>
        /// <param name="providerState">Provider state description. All provider states are verified if this is null</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithFilter(string description = null, string providerState = null)
        {
            // TODO: Allow env vars to specify description and provider state filters like the old version did
            // TODO: Support the 'no state' option
            this.provider.SetFilterInfo(description, providerState, null);

            return this;
        }

        /// <summary>
        /// Set the timeout for all requests to the provider
        /// </summary>
        /// <param name="timeout">Timeout</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithRequestTimeout(TimeSpan timeout)
        {
            this.timeout = timeout;
            return this;
        }

        /// <summary>
        /// Disable certificate verification for HTTPS requests
        /// </summary>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithSslVerificationDisabled()
        {
            this.disableSslVerification = true;
            return this;
        }

        /// <summary>
        /// Add a header which will be used in all calls from the verifier to the provider, for example
        /// an Authorization header with a valid auth token
        /// </summary>
        /// <param name="name">Header name</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithCustomHeader(string name, string value)
        {
            this.provider.AddCustomHeader(name, value);
            return this;
        }

        /// <summary>
        /// Verify provider interactions
        /// </summary>
        /// <exception cref="PactFailureException">Verification failed</exception>
        public void Verify()
        {
            this.provider.SetVerificationOptions(this.disableSslVerification, this.timeout);

            this.config.WriteLine("Starting verification...");
            this.provider.Execute();
        }
    }
}
