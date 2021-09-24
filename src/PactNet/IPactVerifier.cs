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
        IPactVerifier FromPactFile(FileInfo pactFile);

        /// <summary>
        /// Verify a pact from a URI
        /// </summary>
        /// <param name="pactUri">Pact file URI</param>
        /// <returns>Fluent builder</returns>
        IPactVerifier FromPactUri(Uri pactUri);

        /// <summary>
        /// Use the pact broker to retrieve pact files
        /// </summary>
        /// <param name="brokerBaseUri">Base URI for the broker</param>
        /// <param name="uriOptions">Options for calling the pact broker</param>
        /// <param name="enablePending">Enable pending pacts?</param>
        /// <param name="consumerVersionTags">Consumer tag versions to retrieve</param>
        /// <param name="includeWipPactsSince">Include WIP pacts since the given filter</param>
        /// <returns>Fluent builder</returns>
        IPactVerifier FromPactBroker(Uri brokerBaseUri,
                                     PactUriOptions uriOptions = null,
                                     bool enablePending = false,
                                     IEnumerable<string> consumerVersionTags = null,
                                     string includeWipPactsSince = null);

        /// <summary>
        /// Set up the provider state setup URL so the service can configure states
        /// </summary>
        /// <param name="providerStateUri">Provider state setup URI</param>
        /// <returns>Fluent builder</returns>
        IPactVerifier WithProviderStateUrl(Uri providerStateUri);

        /// <summary>
        /// Filter the interactions to only those matching the given description and/or provider state
        /// </summary>
        /// <param name="description">Interaction description. All interactions are verified if this is null</param>
        /// <param name="providerState">Provider state description. All provider states are verified if this is null</param>
        /// <returns>Fluent builder</returns>
        IPactVerifier WithFilter(string description = null, string providerState = null);

        /// <summary>
        /// Publish results to the pact broker
        /// </summary>
        /// <param name="providerVersion">Provider version</param>
        /// <param name="providerTags">Optional tags to add to the verification</param>
        /// <returns>Fluent builder</returns>
        IPactVerifier WithPublishedResults(string providerVersion, IEnumerable<string> providerTags = null);

        /// <summary>
        /// Alter the log level from the default value
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Fluent builder</returns>
        IPactVerifier WithLogLevel(PactLogLevel level);

        /// <summary>
        /// Verify provider interactions
        /// </summary>
        void Verify();
    }
}
