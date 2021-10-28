using System;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Configured the messaging scenarios
    /// </summary>
    public interface IPactVerifierMessagingProvider
    {
        /// <summary>
        /// Configure provider messages
        /// </summary>
        /// <param name="scenarios">Scenario configuration</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierProvider WithProviderMessages(Action<IMessageScenarios> scenarios);
    }
}
