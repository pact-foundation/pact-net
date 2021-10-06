using System;
using System.IO;

namespace PactNet.Verifier
{
    /// <summary>
    /// Pact verifier consumer
    /// </summary>
    public interface IPactVerifierConsumer
    {
        /// <summary>
        /// Verify a pact file directly
        /// </summary>
        /// <param name="pactFile">Pact file path</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierPair FromPactFile(FileInfo pactFile);

        /// <summary>
        /// Verify a pact from a URI
        /// </summary>
        /// <param name="pactUri">Pact file URI</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierPair FromPactUri(Uri pactUri);

        /// <summary>
        /// Use the pact broker to retrieve pact files with default options
        /// </summary>
        /// <param name="brokerBaseUri">Base URI for the broker</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierPair FromPactBroker(Uri brokerBaseUri);

        /// <summary>
        /// Use the pact broker to retrieve pact files
        /// </summary>
        /// <param name="brokerBaseUri">Base URI for the broker</param>
        /// <param name="configure">Configure pact broker options</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierPair FromPactBroker(Uri brokerBaseUri, Action<IPactBrokerOptions> configure);
    }
}
