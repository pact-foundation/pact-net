using System;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Defines the message scenario builder
    /// </summary>
    public static class MessageScenarioBuilder
    {
        /// <summary>
        /// Set the description of the scenario
        /// </summary>
        /// <param name="description">the name of the interaction</param>
        /// <returns>The content message fluent builder</returns>
        public static IMessageScenarioContentBuilder WhenReceiving(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            return new MessageScenarioContentBuilder(description);
        }
    }
}
