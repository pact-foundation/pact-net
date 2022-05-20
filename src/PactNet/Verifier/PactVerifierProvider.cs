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
        private readonly IVerifierProvider provider;
        private readonly PactVerifierConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifierProvider"/> class.
        /// </summary>
        /// <param name="provider">Verifier provider</param>
        /// <param name="config">Pact verifier config</param>
        public PactVerifierProvider(IVerifierProvider provider, PactVerifierConfig config)
        {
            this.provider = provider;
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

            this.provider.AddFileSource(pactFile);

            return new PactVerifierSource(this.provider, this.config);
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

            this.provider.AddDirectorySource(directory);
            this.provider.SetConsumerFilters(consumers);

            return new PactVerifierSource(this.provider, this.config);
        }

        /// <summary>
        /// Verify a pact from a URI
        /// </summary>
        /// <param name="pactUri">Pact file URI</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithUriSource(Uri pactUri)
            => this.WithUriSource(pactUri, _ => { });

        /// <summary>
        /// Verify a pact from a URI
        /// </summary>
        /// <param name="pactUri">Pact file URI</param>
        /// <param name="configure">Configure URI options</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithUriSource(Uri pactUri, Action<IPactUriOptions> configure)
        {
            Guard.NotNull(pactUri, nameof(pactUri));

            var options = new PactUriOptions(this.provider, pactUri);
            configure?.Invoke(options);
            options.Apply();

            return new PactVerifierSource(this.provider, this.config);
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

            var options = new PactBrokerOptions(this.provider, brokerBaseUri);
            configure?.Invoke(options);
            options.Apply();

            return new PactVerifierSource(this.provider, this.config);
        }
    }
}
