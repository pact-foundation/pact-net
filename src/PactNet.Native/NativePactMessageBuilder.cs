using System;

using Newtonsoft.Json;

using PactNet.Native.Exceptions;
using PactNet.Native.Models;

namespace PactNet.Native
{
    /// <summary>
    /// Mock request builder
    /// </summary>
    public class NativePactMessageBuilder : IPactMessageBuilderV3
    {
        private readonly IMockServer server;
        private readonly MessagePactHandle pact;
        private readonly PactConfig _config;
        private readonly MessageHandle message;
        private readonly JsonSerializerSettings defaultSettings;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativePactMessageBuilder"/> class.
        /// </summary>
        /// <param name="server">Mock server</param>
        /// <param name="pact">the pact handle</param>
        /// <param name="config">the pact configuration</param>
        /// <param name="defaultSettings">Default JSON serializer settings</param>
        internal NativePactMessageBuilder(IMockServer server, MessagePactHandle pact, PactConfig config, JsonSerializerSettings defaultSettings = null)
        {
            this.pact = pact;
            _config = config;
            this.server = server;
            this.message = server.NewMessage(pact, "default message");
            this.defaultSettings = defaultSettings;
        }

        #region IPactMessageBuilderV3 explicit implementation

        IPactMessageBuilderV3 IPactMessageBuilderV3.ExpectsToReceive(string description)
        {
            server.MessageExpectsToReceive(message, description);

            return this;
        }

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        IPactMessageBuilderV3 IPactMessageBuilderV3.Given(string providerState)
        {
            this.server.MessageGiven(message, providerState);

            return this;
        }

        /*
         *         /// <summary>
        /// Set the metadata
        /// </summary>
        /// <param name="metadata">Dynamic metadata</param>
        /// <returns>Fluent builder</returns>
        IPactMessageBuilderV3 WithMetadata(dynamic metadata);

        /// <summary>
        /// Set the content
        /// </summary>
        /// <param name="content">Dynamic content</param>
        /// <returns>Fluent builder</returns>
        IPactMessageBuilderV3 WithContent(dynamic content);
         */

        /// <summary>
        /// Set the metadata
        /// </summary>
        /// <param name="key">key of the metadata</param>
        /// <param name="vaule">value of the metadata</param>
        /// <returns>Fluent builder</returns>
        IPactMessageBuilderV3 IPactMessageBuilderV3.WithMetadata(string key, string value)
        {
            this.server.MessageWithMetadata(message, key, value);

            return this;
        }

        /// <summary>
        /// Set the content
        /// </summary>
        /// <param name="content">Dynamic content</param>
        /// <returns>Fluent builder</returns>
        IPactMessageBuilderV3 IPactMessageBuilderV3.WithContent(dynamic content)
        {
            this.server.MessageWithContents(message, "application/json", JsonConvert.SerializeObject(content), 100);

            return this;
        }

        #endregion

        /// <summary>
        /// Build the pact file
        /// </summary>
        public void Build()
        {
            server.WriteMessagePactFile(pact, _config.PactDir, true);
        }

        /// <summary>
        /// Verify a message is read and handled correctly
        /// </summary>
        /// <param name="handler">The method using the message</param>
        public void Verify<T>(Action<T> handler)
        {
            try
            {
                var content = JsonConvert.DeserializeObject<NativeMessage>(server.MessageReify(message));

                var messageReified = JsonConvert.DeserializeObject<T>(content.Contents.ToString(), defaultSettings);

                handler(messageReified);
            }
            catch (Exception e)
            {
                throw new PactMessageConsumerVerificationException($"The message {message} could not be verified by the consumer handler", e);
            }
        }
    }
}
