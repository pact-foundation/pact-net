using System.Collections.Generic;
using PactNet.Native.Internal;
using PactNet.Verifier;

namespace PactNet.Native.Verifier
{
    /// <summary>
    /// Native pact verifier provider type state
    /// </summary>
    internal class NativePactVerifierProvider : IPactVerifierProvider
    {
        private readonly IDictionary<string, string> verifierArgs;
        private readonly PactVerifierConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativePactVerifierProvider"/> class.
        /// </summary>
        /// <param name="verifierArgs">Verifier args to populate</param>
        /// <param name="config">Pact verifier config</param>
        public NativePactVerifierProvider(IDictionary<string, string> verifierArgs, PactVerifierConfig config)
        {
            this.verifierArgs = verifierArgs;
            this.config = config;
        }

        /// <summary>
        /// Set the consumer name
        /// </summary>
        /// <param name="consumerName">Consumer name</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierConsumer HonoursPactWith(string consumerName)
        {
            this.verifierArgs.AddOption("--filter-consumer", consumerName);

            return new NativePactVerifierConsumer(this.verifierArgs, this.config);
        }
    }
}
