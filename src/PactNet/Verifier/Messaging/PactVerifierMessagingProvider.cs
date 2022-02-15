using System;
using PactNet.Internal;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Configured the messaging scenarios
    /// </summary>
    internal class PactVerifierMessagingProvider : IPactVerifierMessagingProvider
    {
        private readonly IVerifierProvider provider;
        private readonly IMessageScenarios scenarios;
        private readonly PactVerifierConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifierMessagingProvider"/> class.
        /// </summary>
        /// <param name="provider">Pact verifier provider</param>
        /// <param name="scenarios">Message scenarios</param>
        /// <param name="config">Pact verifier config</param>
        public PactVerifierMessagingProvider(IVerifierProvider provider, IMessageScenarios scenarios, PactVerifierConfig config)
        {
            this.provider = provider;
            this.scenarios = scenarios;
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

            scenarios(this.scenarios);

            return new PactVerifierProvider(this.provider, this.config);
        }
    }
}
