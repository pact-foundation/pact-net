using System.Collections.Generic;
using PactNet.Internal;

namespace PactNet.Verifier
{
    /// <summary>
    /// Native pact verifier provider type state
    /// </summary>
    internal class PactVerifierProvider : IPactVerifierProvider
    {
        private readonly IDictionary<string, string> verifierArgs;
        private readonly PactVerifierConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifierProvider"/> class.
        /// </summary>
        /// <param name="verifierArgs">Verifier args to populate</param>
        /// <param name="config">Pact verifier config</param>
        public PactVerifierProvider(IDictionary<string, string> verifierArgs, PactVerifierConfig config)
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

            return new PactVerifierConsumer(this.verifierArgs, this.config);
        }
    }
}
