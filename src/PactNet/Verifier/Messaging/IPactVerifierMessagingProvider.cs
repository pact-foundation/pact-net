using System;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Configured the messaging scenarios
    /// </summary>
    public interface IPactVerifierMessagingProvider : IPactVerifierProvider
    {
        /// <summary>
        /// Configure provider messages
        /// </summary>
        /// <param name="scenarios">Scenario configuration</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierMessagingProvider WithProviderMessages(Action<IMessageScenarios> scenarios);
    }
}
