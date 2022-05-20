using System;
using System.IO;

namespace PactNet.Verifier
{
    /// <summary>
    /// Pact verifier provider
    /// </summary>
    public interface IPactVerifierProvider
    {
        /// <summary>
        /// Verify a pact file directly
        /// </summary>
        /// <param name="pactFile">Pact file path</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierSource WithFileSource(FileInfo pactFile);

        /// <summary>
        /// Verify all pacts in the given directory which match the given consumers (or all pact files if no consumers are specified)
        /// </summary>
        /// <param name="directory">Directory containing the pact files</param>
        /// <param name="consumers">(Optional) Consumer names to filter on</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierSource WithDirectorySource(DirectoryInfo directory, params string[] consumers);

        /// <summary>
        /// Verify a pact from a URI
        /// </summary>
        /// <param name="pactUri">Pact file URI</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierSource WithUriSource(Uri pactUri);

        /// <summary>
        /// Verify a pact from a URI
        /// </summary>
        /// <param name="pactUri">Pact file URI</param>
        /// <param name="configure">Configure URI options</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierSource WithUriSource(Uri pactUri, Action<IPactUriOptions> configure);

        /// <summary>
        /// Use the pact broker to retrieve pact files with default options
        /// </summary>
        /// <param name="brokerBaseUri">Base URI for the broker</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierSource WithPactBrokerSource(Uri brokerBaseUri);

        /// <summary>
        /// Use the pact broker to retrieve pact files
        /// </summary>
        /// <param name="brokerBaseUri">Base URI for the broker</param>
        /// <param name="configure">Configure pact broker options</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierSource WithPactBrokerSource(Uri brokerBaseUri, Action<IPactBrokerOptions> configure);
    }
}
