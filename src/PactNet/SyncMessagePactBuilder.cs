using System;
using PactNet.Drivers;
using PactNet.Interop;

namespace PactNet
{
    /// <summary>
    /// Mock synchronous (request/response) message pact builder
    /// </summary>
    internal class SyncMessagePactBuilder : ISyncMessagePactBuilderV4
    {
        private readonly IMessagePactDriver driver;
        private readonly PactConfig config;
        private readonly PactSpecification version;

        /// <summary>
        /// Initialises a new instance of the <see cref="SyncMessagePactBuilder"/> class.
        /// </summary>
        /// <param name="pact">Pact driver</param>
        /// <param name="config">the message pact configuration</param>
        /// <param name="version">Pact specification version</param>
        internal SyncMessagePactBuilder(IMessagePactDriver pact, PactConfig config, PactSpecification version)
        {
            this.driver = pact ?? throw new ArgumentNullException(nameof(pact));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.version = version;
        }

        #region ISyncMessagePactBuilderV4 explicit implementation

        /// <inheritdoc cref="ISyncMessagePactBuilderV4"/>
        ISyncMessageBuilderV4 ISyncMessagePactBuilderV4.ExpectsToReceive(string description)
            => ExpectsToReceive(description);

        /// <inheritdoc cref="ISyncMessagePactBuilderV4"/>
        ISyncMessagePactBuilderV4 ISyncMessagePactBuilderV4.WithPactMetadata(string @namespace, string name, string value)
            => WithPactMetadata(@namespace, name, value);

        #endregion

        /// <summary>
        /// Add a new synchronous message to the message pact
        /// </summary>
        /// <param name="description">Message description</param>
        /// <returns>Fluent builder</returns>
        internal SyncMessageBuilder ExpectsToReceive(string description)
        {
            ISyncMessageInteractionDriver messageDriver = this.driver.NewSyncMessageInteraction(description);
            return new SyncMessageBuilder(messageDriver, this.config, this.version);
        }

        /// <summary>
        /// Add a new metadata to the message pact
        /// </summary>
        /// <param name="namespace">the parent configuration section</param>
        /// <param name="name">the metadata field value</param>
        /// <param name="value">the metadata field value</param>
        /// <returns>Fluent builder</returns>
        internal SyncMessagePactBuilder WithPactMetadata(string @namespace, string name, string value)
        {
            this.driver.WithMessagePactMetadata(@namespace, name, value);
            return this;
        }
    }
}
