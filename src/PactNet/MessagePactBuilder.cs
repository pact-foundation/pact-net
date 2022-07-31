using System;
using PactNet.Drivers;

namespace PactNet
{
    /// <summary>
    /// Mock request builder
    /// </summary>
    internal class MessagePactBuilder : IMessagePactBuilderV3
    {
        private readonly IMessagePactDriver driver;
        private readonly PactConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="MessagePactBuilder"/> class.
        /// </summary>
        /// <param name="pact">Pact driver</param>
        /// <param name="config">the message pact configuration</param>
        internal MessagePactBuilder(IMessagePactDriver pact, PactConfig config)
        {
            this.driver = pact ?? throw new ArgumentNullException(nameof(pact));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        #region IMessagePactBuilderV3 explicit implementation

        /// <inheritdoc cref="IMessagePactBuilderV3"/>
        IMessageBuilderV3 IMessagePactBuilderV3.ExpectsToReceive(string description)
            => ExpectsToReceive(description);

        /// <inheritdoc cref="IMessagePactBuilderV3"/>
        IMessagePactBuilderV3 IMessagePactBuilderV3.WithPactMetadata(string @namespace, string name, string value)
            => WithPactMetadata(@namespace, name, value);

        #endregion

        /// <summary>
        /// Add a new message to the message pact
        /// </summary>
        /// <param name="description">Message description</param>
        /// <returns>Fluent builder</returns>
        internal IMessageBuilderV3 ExpectsToReceive(string description)
        {
            IMessageInteractionDriver messageDriver = this.driver.NewMessageInteraction(description);
            return new MessageBuilder(messageDriver, this.config);
        }

        /// <summary>
        /// Add a new metadata to the message pact
        /// </summary>
        /// <param name="namespace">the parent configuration section</param>
        /// <param name="name">the metadata field value</param>
        /// <param name="value">the metadata field value</param>
        /// <returns>Fluent builder</returns>
        internal IMessagePactBuilderV3 WithPactMetadata(string @namespace, string name, string value)
        {
            this.driver.WithMessagePactMetadata(@namespace, name, value);
            return this;
        }
    }
}
