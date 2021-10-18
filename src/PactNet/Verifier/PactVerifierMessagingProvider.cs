using System;
using System.Collections.Generic;
using PactNet.Internal;
using PactNet.Verifier.Messaging;

namespace PactNet.Verifier
{
    /// <summary>
    /// Configured the messaging scenarios
    /// </summary>
    internal class PactVerifierMessagingProvider : IPactVerifierMessagingProvider
    {
        private readonly IDictionary<string, string> verifierArgs;
        private readonly PactVerifierConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifierMessagingProvider"/> class.
        /// </summary>
        /// <param name="verifierArgs">Verifier arguments</param>
        /// <param name="config">Pact verifier config</param>
        public PactVerifierMessagingProvider(IDictionary<string, string> verifierArgs, PactVerifierConfig config)
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
            return new PactVerifierConsumer(this.verifierArgs, this.config);
        }

        /// <summary>
        /// Configure provider messages
        /// </summary>
        /// <param name="scenarios">Scenario configuration</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierMessagingProvider WithProviderMessages(Action<IMessageScenariosFactory> scenarios)
        {
            Guard.NotNull(scenarios, nameof(scenarios));

            scenarios(new MessageScenariosFactory());

            return this;
        }
    }
}
