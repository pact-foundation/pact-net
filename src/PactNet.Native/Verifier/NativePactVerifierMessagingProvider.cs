using System;
using System.Collections.Generic;
using PactNet.Native.Internal;
using PactNet.Verifier;
using PactNet.Verifier.Messaging;

namespace PactNet.Native.Verifier
{
    /// <summary>
    /// Configured the messaging scenarios
    /// </summary>
    internal class NativePactVerifierMessagingProvider : IPactVerifierMessagingProvider
    {
        private readonly IDictionary<string, string> verifierArgs;
        private readonly PactVerifierConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativePactVerifierMessagingProvider"/> class.
        /// </summary>
        /// <param name="verifierArgs">Verifier arguments</param>
        /// <param name="config">Pact verifier config</param>
        public NativePactVerifierMessagingProvider(IDictionary<string, string> verifierArgs, PactVerifierConfig config)
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
            return new NativePactVerifierConsumer(this.verifierArgs, this.config);
        }

        /// <summary>
        /// Configure provider messages
        /// </summary>
        /// <param name="scenarios">Scenario configuration</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierMessagingProvider WithProviderMessages(Action<IMessageScenarios> scenarios)
        {
            Guard.NotNull(scenarios, nameof(scenarios));

            scenarios(new MessageScenarios());

            return this;
        }
    }
}
