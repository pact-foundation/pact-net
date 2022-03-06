using System;
using PactNet.Exceptions;

namespace PactNet.Verifier
{
    /// <summary>
    /// Configured pact verifier source
    /// </summary>
    public interface IPactVerifierSource
    {
        /// <summary>
        /// Set up the provider state setup URL so the service can configure states
        /// </summary>
        /// <param name="providerStateUri">Provider state setup URI</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierSource WithProviderStateUrl(Uri providerStateUri);

        /// <summary>
        /// Filter the interactions to only those matching the given description and/or provider state
        /// </summary>
        /// <param name="description">Interaction description. All interactions are verified if this is null</param>
        /// <param name="providerState">Provider state description. All provider states are verified if this is null</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierSource WithFilter(string description = null, string providerState = null);

        /// <summary>
        /// Set the timeout for all requests to the provider
        /// </summary>
        /// <param name="timeout">Timeout</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierSource WithRequestTimeout(TimeSpan timeout);

        /// <summary>
        /// Disable certificate verification for HTTPS requests
        /// </summary>
        /// <returns>Fluent builder</returns>
        IPactVerifierSource WithSslVerificationDisabled();

        /// <summary>
        /// Add a header which will be used in all calls from the verifier to the provider, for example
        /// an Authorization header with a valid auth token
        /// </summary>
        /// <param name="name">Header name</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierSource WithCustomHeader(string name, string value);

        /// <summary>
        /// Verify provider interactions
        /// </summary>
        /// <exception cref="PactFailureException">Verification failed</exception>
        void Verify();
    }
}
