using System;

using Newtonsoft.Json;

using PactNet.Exceptions;
using PactNet.Native.Models;

namespace PactNet.Native
{
    /// <summary>
    /// Mock request builder
    /// </summary>
    public class NativePactMessageBuilder : IPactMessageBuilderV3
    {
        private readonly IMessageMockServer server;
        private readonly MessagePactHandle pact;
        private readonly PactConfig config;
        private MessageHandle message;
        private readonly JsonSerializerSettings defaultSettings;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativePactMessageBuilder"/> class.
        /// </summary>
        /// <param name="server">Mock server</param>
        /// <param name="pact">the pact handle</param>
        /// <param name="config">the pact configuration</param>
        /// <param name="defaultSettings">Default JSON serializer settings</param>
        internal NativePactMessageBuilder(IMessageMockServer server, MessagePactHandle pact, PactConfig config, JsonSerializerSettings defaultSettings = null)
        {
            this.pact = pact;
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.server = server ?? throw new ArgumentNullException(nameof(server));
            this.defaultSettings = defaultSettings;
        }

        #region IPactMessageBuilderV3 explicit implementation

        /// <inheritdoc cref="IPactMessageBuilderV3"/>
        IMessageBuilderV3 IPactMessageBuilderV3.ExpectsToReceive(string description)
            => ExpectsToReceive(description);

        /// <inheritdoc cref="IPactMessageBuilderV3"/>
        void IPactMessageBuilder.Verify<T>(Action<T> handler)
            => Verify(handler);

        #endregion

        #region Internal Methods

        /// <summary>
        /// Add a new message to the pact
        /// </summary>
        /// <param name="description">Message description</param>
        /// <returns>Fluent builder</returns>
        internal IMessageBuilderV3 ExpectsToReceive(string description)
        {
            this.message = server.NewMessage(pact, "default message");

            server.ExpectsToReceive(this.message, description);

            return new NativeMessageBuilder(server, this.message);
        }

        /// <summary>
        /// Verify a message is read and handled correctly and write the pact
        /// </summary>
        /// <param name="handler">The method using the message</param>
        internal void Verify<T>(Action<T> handler)
        {
            try
            {
                var content = JsonConvert.DeserializeObject<NativeMessage>(server.Reify(message));

                var messageReified = JsonConvert.DeserializeObject<T>(content.Contents.ToString(), defaultSettings);

                handler(messageReified);

                server.WriteMessagePactFile(pact, config.PactDir, true);
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
