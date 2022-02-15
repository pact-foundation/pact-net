using System;
using Newtonsoft.Json;
using PactNet.Verifier.Messaging;

namespace PactNet.Verifier
{
    /// <summary>
    /// Pact verifier
    /// </summary>
    public interface IPactVerifier
    {
        /// <summary>
        /// Set the provider details
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <param name="pactUri">URI of the running service</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierProvider ServiceProvider(string providerName, Uri pactUri);

        /// <summary>
        /// Set the provider details of a messaging provider
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierMessagingProvider MessagingProvider(string providerName);

        /// <summary>
        /// Set the provider details of a messaging provider
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <param name="settings">Default JSON serialisation settings</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierMessagingProvider MessagingProvider(string providerName, JsonSerializerSettings settings);
    }
}
