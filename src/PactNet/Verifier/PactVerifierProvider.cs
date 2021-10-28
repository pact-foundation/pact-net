using System;
using System.IO;
using PactNet.Internal;

namespace PactNet.Verifier
{
    /// <summary>
    /// Native pact verifier provider type state
    /// </summary>
    internal class PactVerifierProvider : IPactVerifierProvider
    {
        private readonly IVerifierArguments verifierArgs;
        private readonly PactVerifierConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifierProvider"/> class.
        /// </summary>
        /// <param name="verifierArgs">Verifier args to populate</param>
        /// <param name="config">Pact verifier config</param>
        public PactVerifierProvider(IVerifierArguments verifierArgs, PactVerifierConfig config)
        {
            this.verifierArgs = verifierArgs;
            this.config = config;
        }

        /// <summary>
        /// Verify a pact file directly
        /// </summary>
        /// <param name="pactFile">Pact file path</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithFileSource(FileInfo pactFile)
        {
            Guard.NotNull(pactFile, nameof(pactFile));

            this.verifierArgs.AddOption("--file", pactFile.FullName);

            return new PactVerifierSource(this.verifierArgs, new InteropVerifierProvider(), this.config);
        }

        /// <summary>
        /// Verify all pacts in the given directory which match the given consumers (or all pact files if no consumers are specified)
        /// </summary>
        /// <param name="directory">Directory containing the pact files</param>
        /// <param name="consumers">(Optional) Consumer names to filter on</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithDirectorySource(DirectoryInfo directory, params string[] consumers)
        {
            Guard.NotNull(directory, nameof(directory));
            Guard.NotNull(consumers, nameof(consumers));

            this.verifierArgs.AddOption("--dir", directory.FullName);

            foreach (string consumer in consumers)
            {
                this.verifierArgs.AddOption("--filter-consumer", consumer);
            }

            return new PactVerifierSource(this.verifierArgs, new InteropVerifierProvider(), this.config);
        }

        /// <summary>
        /// Verify a pact from a URI
        /// </summary>
        /// <param name="pactUri">Pact file URI</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithUriSource(Uri pactUri)
        {
            Guard.NotNull(pactUri, nameof(pactUri));

            this.verifierArgs.AddOption("--url", pactUri.ToString());

            return new PactVerifierSource(this.verifierArgs, new InteropVerifierProvider(), this.config);
        }

        /// <summary>
        /// Use the pact broker to retrieve pact files with default options
        /// </summary>
        /// <param name="brokerBaseUri">Base URI for the broker</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithPactBrokerSource(Uri brokerBaseUri)
            => this.WithPactBrokerSource(brokerBaseUri, _ => { });

        /// <summary>
        /// Use the pact broker to retrieve pact files
        /// </summary>
        /// <param name="brokerBaseUri">Base URI for the broker</param>
        /// <param name="configure">Configure pact broker options</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithPactBrokerSource(Uri brokerBaseUri, Action<IPactBrokerOptions> configure)
        {
            Guard.NotNull(brokerBaseUri, nameof(brokerBaseUri));
            Guard.NotNull(configure, nameof(configure));

            this.verifierArgs.AddOption("--broker-url", brokerBaseUri.ToString());

            var options = new PactBrokerOptions(this.verifierArgs);
            configure?.Invoke(options);

            return new PactVerifierSource(this.verifierArgs, new InteropVerifierProvider(), this.config);
        }
    }
}
