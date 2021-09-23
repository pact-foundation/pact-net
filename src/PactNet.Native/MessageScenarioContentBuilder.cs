using System;
using System.Collections.Generic;

namespace PactNet.Native
{
    /// <summary>
    /// Defines the message scenario builder
    /// </summary>
    internal class MessageScenarioContentBuilder : IMessageScenarioContentBuilder
    {
        private readonly string description;
        private readonly List<Scenario> scenarios;
        private dynamic metadata;

        internal MessageScenarioContentBuilder(string description, List<Scenario> scenarios)
        {
            this.description = !string.IsNullOrWhiteSpace(description) ? description : throw new ArgumentNullException(nameof(description));
            this.scenarios = scenarios ?? throw new ArgumentNullException(nameof(scenarios));
        }

        /// <inheritdoc />
        public IMessageScenarioContentBuilder WithMetadata(dynamic metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            this.metadata = metadata;
            return this;
        }

        /// <inheritdoc />
        public void WithContent(Func<dynamic> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            AddScenario(action);
        }

        /// <inheritdoc />
        public void WithContent(dynamic messageContent)
        {
            if (messageContent == null)
            {
                throw new ArgumentNullException(nameof(messageContent));
            }

            AddScenario(() => messageContent);
        }

        /// <summary>
        /// Add a scenario
        /// </summary>
        /// <param name="action">the action that will publish the message</param>
        private void AddScenario(Func<dynamic> action)
        {
            var scenario = metadata == null
                ? new Scenario(description, action)
                : new Scenario(description, action, metadata);

            scenarios.Add(scenario);
        }
    }
}
