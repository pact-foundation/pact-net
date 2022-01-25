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
        /// Verify provider interactions
        /// </summary>
        /// <exception cref="PactFailureException">Verification failed</exception>
        void Verify();
    }
}
