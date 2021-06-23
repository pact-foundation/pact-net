using System;
using System.Collections.Generic;

namespace PactNet.Native
{
    /// <summary>
    /// Defines the message scenario builder
    /// </summary>
    public class MessageScenarioBuilder : IMessageScenarioBuilder
    {
        private static IMessageScenarioBuilder _instance;

        /// <summary>
        /// The message scenario builder singleton instance
        /// </summary>
        public static IMessageScenarioBuilder Instance => _instance ??= new MessageScenarioBuilder();

        /// <summary>
        /// The available scenarios
        /// </summary>
        public readonly Dictionary<string, Func<dynamic>> Scenarios;

        private string _descriptionAdded;

        private MessageScenarioBuilder()
        {
            Scenarios = new Dictionary<string, Func<dynamic>>();
        }

        /// <inheritdoc />
        public IMessageScenarioBuilder WhenReceiving(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            _descriptionAdded = description;

            return this;
        }

        /// <inheritdoc />
        public void WillPublishMessage(Func<dynamic> action)
        {
            Scenarios[_descriptionAdded] = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <inheritdoc />
        public dynamic InvokeScenario(string description)
        {
            return Scenarios.TryGetValue(description, out _) ? Scenarios[description].Invoke() : null;
        }
    }
}
