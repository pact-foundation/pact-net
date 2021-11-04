using System;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Defines the message scenario builder
    /// </summary>
    internal class MessageScenarioContentBuilder : IMessageScenarioContentBuilder
    {
        private readonly string description;
        private dynamic metadataInternal;

        /// <summary>
        /// Creates an instance of <see cref="MessageScenarioContentBuilder"/>
        /// </summary>
        /// <param name="description">the description of the scenario</param>
        internal MessageScenarioContentBuilder(string description)
        {
            this.description = !string.IsNullOrWhiteSpace(description) ? description : throw new ArgumentNullException(nameof(description));
        }

        /// <summary>
        /// Set the metadata of the message content
        /// </summary>
        /// <param name="metadata">the metadata</param>
        /// <returns>Fluent builder</returns>
        public IMessageScenarioContentBuilder WithMetadata(dynamic metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            this.metadataInternal = metadata;
            return this;
        }

        /// <summary>
        /// Set the action of the scenario
        /// </summary>
        /// <param name="action">the function invoked</param>
        public IScenario WithContent(Func<dynamic> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return CreateScenario(action);
        }

        /// <summary>
        /// Set the object returned by the scenario
        /// </summary>
        /// <param name="messageContent">the message content</param>
        public IScenario WithContent(dynamic messageContent)
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
        private IScenario CreateScenario(Func<dynamic> action)
        {
            var scenario = metadataInternal == null
                ? new Scenario(description, action)
                : new Scenario(description, action, metadataInternal);

            return scenario;
        }
    }
}
