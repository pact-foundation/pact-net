using System;

namespace PactNet.Verifier
{
    /// <summary>
    /// Configured pact verifier pair
    /// </summary>
    public interface IPactVerifierPair
    {
        /// <summary>
        /// Set up the provider state setup URL so the service can configure states
        /// </summary>
        /// <param name="providerStateUri">Provider state setup URI</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierPair WithProviderStateUrl(Uri providerStateUri);

        /// <summary>
        /// Filter the interactions to only those matching the given description and/or provider state
        /// </summary>
        /// <param name="description">Interaction description. All interactions are verified if this is null</param>
        /// <param name="providerState">Provider state description. All provider states are verified if this is null</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierPair WithFilter(string description = null, string providerState = null);

        /// <summary>
        /// Alter the log level from the default value
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierPair WithLogLevel(PactLogLevel level);

        /// <summary>
        /// Using messaging interaction model type of provider
        /// </summary>
        /// <returns>Fluent builder</returns>
        IPactVerifierMessagingScenario WithProviderMessages();

        /// <summary>
        /// Verify provider interactions
        /// </summary>
        void Verify();
    }
}
