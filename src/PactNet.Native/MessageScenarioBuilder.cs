using System;

namespace PactNet.Native
{
    /// <summary>
    /// Defines the message scenario builder
    /// </summary>
    public class MessageScenarioBuilder : IMessageScenarioBuilder
    {
        /// <summary>
        /// Handles to create a new scenario with scenario builder
        /// </summary>
        public static IMessageScenarioBuilder WillPublishMessage => new MessageScenarioBuilder();

        /// <inheritdoc />
        public IMessageScenarioContentBuilder WhenReceiving(string description)
        {
            ValidateDescription(description);

            return new MessageScenarioContentBuilder(description, Scenarios.All);
        }

        /// <summary>
        /// Validates we can add a scenario description
        /// </summary>
        /// <param name="description">The description to add</param>
        private void ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (Scenarios.Exist(description))
            {
                throw new InvalidOperationException($"Scenario called \"{description}\" has already been added");
            }
        }
    }
}
