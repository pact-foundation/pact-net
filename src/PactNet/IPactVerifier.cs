using System;
using System.Collections.Generic;
using System.IO;

namespace PactNet
{
    /// <summary>
    /// Pact verifier
    /// </summary>
    public interface IPactVerifier
    {
        /// <summary>
        /// Set up the provider state setup path so the service can configure states
        /// </summary>
        /// <param name="providerStatePath">Provider state setup path</param>
        /// <returns>Fluent builder</returns>
        IPactVerifier ProviderState(string providerStatePath);

        /// <summary>
        /// Set the provider details
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <param name="pactUri">URI of the running service</param>
        /// <returns>Fluent builder</returns>
        IPactVerifier ServiceProvider(string providerName, Uri pactUri);

        /// <summary>
        /// Set the consumer name
        /// </summary>
        /// <param name="consumerName">Consumer name</param>
        /// <returns>Fluent builder</returns>
        IPactVerifier HonoursPactWith(string consumerName);

        /// <summary>
        /// Verify a pact file directly
        /// </summary>
        /// <param name="pactFile">Pact file path</param>
        /// <returns>Fluent builder</returns>
        IPactVerifier PactFile(FileInfo pactFile);

        /// <summary>
        /// Verify a pact from a URI
        /// </summary>
        /// <param name="pactUri">Pact file URI</param>
        /// <param name="options">Pact URI options</param>
        /// <param name="providerVersionTags">Provider version tags</param>
        /// <returns>Fluent builder</returns>
        IPactVerifier PactUri(Uri pactUri, PactUriOptions options = null, IEnumerable<string> providerVersionTags = null);

        /// <summary>
        /// Use the pact broker to retrieve pact files
        /// </summary>
        /// <param name="brokerBaseUri">Base URI for the broker</param>
        /// <param name="uriOptions">Options for calling the pact broker</param>
        /// <param name="enablePending">Enable pending pacts?</param>
        /// <param name="consumerVersionTags">Consumer tag versions to retrieve</param>
        /// <param name="includeWipPactsSince">Include WIP pacts since the given filter</param>
        /// <returns>Fluent builder</returns>
        IPactVerifier PactBroker(Uri brokerBaseUri,
                                 PactUriOptions uriOptions = null,
                                 bool enablePending = false,
                                 IEnumerable<string> consumerVersionTags = null,
                                 string includeWipPactsSince = null);

        /// <summary>
        /// Verify provider interactions
        /// </summary>
        /// <param name="description">Interaction description. All interactions are verified if this is null</param>
        /// <param name="providerState">Provider state description. All provider states are verified if this is null</param>
        void Verify(string description = null, string providerState = null);
    }
}