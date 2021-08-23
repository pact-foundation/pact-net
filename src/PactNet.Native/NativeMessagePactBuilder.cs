using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PactNet.Exceptions;
using PactNet.Native.Interop;
using PactNet.Native.Models;

namespace PactNet.Native
{
    /// <summary>
    /// Mock request builder
    /// </summary>
    public class NativeMessagePactBuilder : IMessagePactBuilderV3
    {
        private readonly IMessageMockServer server;
        private readonly MessagePactHandle pact;
        private readonly PactConfig config;
        private MessageHandle message;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativeMessagePactBuilder"/> class.
        /// </summary>
        /// <param name="server">Mock server</param>
        /// <param name="pact">the messagePact handle</param>
        /// <param name="config">the messagePact configuration</param>
        internal NativeMessagePactBuilder(IMessageMockServer server, MessagePactHandle pact, PactConfig config)
        {
            this.pact = pact;
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.server = server ?? throw new ArgumentNullException(nameof(server));
        }

        #region IMessagePactBuilderV3 explicit implementation

        /// <inheritdoc cref="IMessagePactBuilderV3"/>
        IMessageBuilderV3 IMessagePactBuilderV3.ExpectsToReceive(string description)
            => ExpectsToReceive(description);

        /// <inheritdoc cref="IMessagePactBuilderV3"/>
        IMessagePactBuilderV3 IMessagePactBuilderV3.WithPactMetadata(string @namespace, string name, string value)
            => WithPactMetadata(@namespace, name, value);

        #endregion

        #region Internal Methods

        /// <summary>
        /// Add a new message to the messagePact
        /// </summary>
        /// <param name="description">Message description</param>
        /// <returns>Fluent builder</returns>
        internal IMessageBuilderV3 ExpectsToReceive(string description)
        {
            this.message = this.server.NewMessage(pact, "default message");

            this.server.ExpectsToReceive(this.message, description);

            return new NativeMessageBuilder(this.server, this.message);
        }

        /// <summary>
        /// Add a new metadata to the messagePact
        /// </summary>
        /// <param name="namespace">the parent configuration section</param>
        /// <param name="name">the metadata field value</param>
        /// <param name="value">the metadata field value</param>
        /// <returns>Fluent builder</returns>
        internal IMessagePactBuilderV3 WithPactMetadata(string @namespace, string name, string value)
        {
            this.server.WithMessagePactMetadata(this.pact, @namespace, name, value);

            return this;
        }

        /// <summary>
        /// Verify a message is read and handled correctly and write the messagePact
        /// </summary>
        /// <param name="handler">The method using the message</param>
        public void Verify<T>(Action<T> handler)
        {
            try
            {
                var content = JsonConvert.DeserializeObject<NativeMessage>(this.server.Reify(message));

                var messageReified = JsonConvert.DeserializeObject<T>(content.Contents.ToString());

                handler(messageReified);

                this.server.WriteMessagePactFile(pact, config.PactDir, false);
            }
            catch (Exception e)
            {
                throw new PactMessageConsumerVerificationException(
                    $"The message {message} could not be verified by the consumer handler", e);
            }
        }

        /// <summary>
        /// Verify a message is read and handled correctly and write the messagePact
        /// </summary>
        /// <param name="handler">The method using the message</param>
        public async Task VerifyAsync<T>(Func<T, Task> handler)
        {
            try
            {
                var content = JsonConvert.DeserializeObject<NativeMessage>(this.server.Reify(message));

                var messageReified = JsonConvert.DeserializeObject<T>(content.Contents.ToString());

                await handler(messageReified);

                this.server.WriteMessagePactFile(pact, config.PactDir, false);
            }
            catch (Exception e)
            {
                throw new PactMessageConsumerVerificationException(
                    $"The message {message} could not be verified by the consumer handler", e);
            }
        }

        #endregion Internal Methods
    }
}
