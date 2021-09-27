using System;

namespace PactNet.Native
{
    /// <summary>
    /// Defines the message scenario builder
    /// </summary>
    public class MessageScenarioBuilder : IMessageScenarioBuilder
    {
        private MessageScenarioBuilder()
        {
        }

        /// <summary>
        /// Handles to create a new scenario with scenario builder
        /// </summary>
        public static IMessageScenarioBuilder AScenario => new MessageScenarioBuilder();

        /// <inheritdoc />
        public IMessageScenarioContentBuilder WhenReceiving(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            return new MessageScenarioContentBuilder(description);
        }
    }
}
