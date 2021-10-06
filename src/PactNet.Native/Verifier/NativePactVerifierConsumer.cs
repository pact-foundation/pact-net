using System;
using System.Collections.Generic;
using System.IO;
using PactNet.Native.Internal;
using PactNet.Verifier;

namespace PactNet.Native.Verifier
{
    /// <summary>
    /// Native pact verifier consumer type state
    /// </summary>
    internal class NativePactVerifierConsumer : IPactVerifierConsumer
    {
        private readonly IDictionary<string, string> verifierArgs;
        private readonly PactVerifierConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativePactVerifierConsumer"/> class.
        /// </summary>
        /// <param name="verifierArgs">Verifier args to populate</param>
        /// <param name="config">Pact verifier config</param>
        public NativePactVerifierConsumer(IDictionary<string, string> verifierArgs, PactVerifierConfig config)
        {
            this.verifierArgs = verifierArgs;
            this.config = config;
        }

        /// <summary>
        /// Verify a pact file directly
        /// </summary>
        /// <param name="pactFile">Pact file path</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierPair FromPactFile(FileInfo pactFile)
        {
            Guard.NotNull(pactFile, nameof(pactFile));

            this.verifierArgs.AddOption("--file", pactFile.FullName);

            return new NativePactVerifierPair(this.verifierArgs, new NativePactVerifier(), this.config);
        }

        /// <summary>
        /// Verify a pact from a URI
        /// </summary>
        /// <param name="pactUri">Pact file URI</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierPair FromPactUri(Uri pactUri)
        {
            Guard.NotNull(pactUri, nameof(pactUri));

            this.verifierArgs.AddOption("--url", pactUri.ToString());

            return new NativePactVerifierPair(this.verifierArgs, new NativePactVerifier(), this.config);
        }

        /// <summary>
        /// Use the pact broker to retrieve pact files with default options
        /// </summary>
        /// <param name="brokerBaseUri">Base URI for the broker</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierPair FromPactBroker(Uri brokerBaseUri)
            => this.FromPactBroker(brokerBaseUri, _ => { });

        /// <summary>
        /// Use the pact broker to retrieve pact files
        /// </summary>
        /// <param name="brokerBaseUri">Base URI for the broker</param>
        /// <param name="configure">Configure pact broker options</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierPair FromPactBroker(Uri brokerBaseUri, Action<IPactBrokerOptions> configure)
        {
            Guard.NotNull(brokerBaseUri, nameof(brokerBaseUri));
            Guard.NotNull(configure, nameof(configure));

            this.verifierArgs.AddOption("--broker-url", brokerBaseUri.ToString());

            var options = new NativePactBrokerOptions(this.verifierArgs);
            configure?.Invoke(options);

            return new NativePactVerifierPair(this.verifierArgs, new NativePactVerifier(), this.config);
        }
    }
}
