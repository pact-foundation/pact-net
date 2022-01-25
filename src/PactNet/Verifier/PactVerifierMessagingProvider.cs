using System;
using PactNet.Internal;
using PactNet.Verifier.Messaging;

namespace PactNet.Verifier
{
    /// <summary>
    /// Configured the messaging scenarios
    /// </summary>
    internal class PactVerifierMessagingProvider : IPactVerifierMessagingProvider
    {
        private readonly IVerifierProvider provider;
        private readonly PactVerifierConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifierMessagingProvider"/> class.
        /// </summary>
        /// <param name="provider">Pact verifier provider</param>
        /// <param name="config">Pact verifier config</param>
        public PactVerifierMessagingProvider(IVerifierProvider provider, PactVerifierConfig config)
        {
            this.provider = provider;
            this.config = config;
        }

        /// <summary>
        /// Configure provider messages
        /// </summary>
        /// <param name="scenarios">Scenario configuration</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierProvider WithProviderMessages(Action<IMessageScenarios> scenarios)
        {
            Guard.NotNull(scenarios, nameof(scenarios));

            scenarios(new MessageScenarios());

            return new PactVerifierProvider(this.provider, this.config);
        }
    }
}
