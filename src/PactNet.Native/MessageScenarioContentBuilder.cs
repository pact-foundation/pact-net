using System;
using PactNet.Models;

namespace PactNet.Native
{
    /// <summary>
    /// Defines the message scenario builder
    /// </summary>
    internal class MessageScenarioContentBuilder : IMessageScenarioContentBuilder
    {
        private readonly string description;
        private dynamic metadata;

        internal MessageScenarioContentBuilder(string description)
        {
            this.description = !string.IsNullOrWhiteSpace(description) ? description : throw new ArgumentNullException(nameof(description));
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
        public Scenario WithContent(Func<dynamic> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return CreateScenario(action);
        }

        /// <inheritdoc />
        public Scenario WithContent(dynamic messageContent)
        {
            if (messageContent == null)
            {
                throw new ArgumentNullException(nameof(messageContent));
            }

            return CreateScenario(() => messageContent);
        }

        /// <summary>
        /// Add a scenario
        /// </summary>
        /// <param name="action">the action that will publish the message</param>
        private Scenario CreateScenario(Func<dynamic> action)
        {
            var scenario = metadata == null
                ? new Scenario(description, action)
                : new Scenario(description, action, metadata);

            return scenario;
        }
    }
}
