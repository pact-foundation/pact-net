using System;
using PactNet.Drivers;
using PactNet.Interop;

namespace PactNet
{
    /// <summary>
    /// Mock request builder
    /// </summary>
    internal class MessagePactBuilder : IMessagePactBuilderV3
    {
        private readonly IAsynchronousMessageDriver driver;
        private readonly PactHandle pact;
        private readonly PactConfig config;
        private InteractionHandle message;

        /// <summary>
        /// Initialises a new instance of the <see cref="MessagePactBuilder"/> class.
        /// </summary>
        /// <param name="driver">Interaction driver</param>
        /// <param name="pact">the message pact handle</param>
        /// <param name="config">the message pact configuration</param>
        internal MessagePactBuilder(IAsynchronousMessageDriver driver, PactHandle pact, PactConfig config)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver));
            this.pact = pact;
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
            this.message = this.driver.NewMessageInteraction(this.pact, description);

            this.driver.ExpectsToReceive(this.message, description);

            return new MessageBuilder(this.driver, this.pact, this.message, this.config);
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
            this.driver.WithMessagePactMetadata(this.pact, @namespace, name, value);

            return this;
        }
    }
}
