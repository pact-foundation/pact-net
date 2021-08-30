using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Native.Interop;

namespace PactNet.Native
{
    /// <summary>
    /// Mock request message builder
    /// </summary>
    public class NativeMessageBuilder : IMessageBuilderV3
    {
        private readonly IMessageMockServer server;
        private readonly MessageHandle message;
        private readonly JsonSerializerSettings defaultSettings;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativeMessagePactBuilder"/> class.
        /// </summary>
        /// <param name="server">Mock server</param>
        /// <param name="message">Message handle</param>
        /// <param name="defaultSettings">Default JSON serialiser settings</param>
        internal NativeMessageBuilder(IMessageMockServer server, MessageHandle message, JsonSerializerSettings defaultSettings)
        {
            this.server = server ?? throw new ArgumentNullException(nameof(server));
            this.message = message;
            this.defaultSettings = defaultSettings;
        }

        #region IMessagePactBuilderV3 explicit implementation

        /// <inheritdoc cref="IMessageBuilderV3"/>
        IMessageBuilderV3 IMessageBuilderV3.Given(string providerState)
            => Given(providerState);

        /// <inheritdoc cref="IMessageBuilderV3"/>
        IMessageBuilderV3 IMessageBuilderV3.Given(string providerState, IDictionary<string, string> parameters)
            => Given(providerState, parameters);

        /// <inheritdoc cref="IMessageBuilderV3"/>
        IMessageBuilderV3 IMessageBuilderV3.WithMetadata(string key, string value)
            => WithMetadata(key, value);

        /// <inheritdoc cref="IMessageBuilderV3"/>
        IMessageBuilderV3 IMessageBuilderV3.WithJsonContent(dynamic content)
            => WithJsonContent(content);

        /// <inheritdoc cref="IMessageBuilderV3"/>
        IMessageBuilderV3 IMessageBuilderV3.WithJsonContent(dynamic content, JsonSerializerSettings settings)
            => WithJsonContent(content, settings);

        #endregion

        #region Internal Methods

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        internal NativeMessageBuilder Given(string providerState)
        {
            this.server.Given(this.message, providerState);

            return this;
        }

        /// <summary>
        /// Add a provider state with one or more parameters
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <param name="parameters">Provider state parameters</param>
        /// <returns>Fluent builder</returns>
        internal NativeMessageBuilder Given(string providerState, IDictionary<string, string> parameters)
        {
            foreach (var param in parameters)
            {
                this.server.GivenWithParam(this.message, providerState, param.Key, param.Value);
            }

            return this;
        }

        /// <summary>
        /// Set the metadata
        /// </summary>
        /// <param name="key">key of the metadata</param>
        /// <param name="value">value of the metadata</param>
        /// <returns>Fluent builder</returns>
        internal NativeMessageBuilder WithMetadata(string key, string value)
        {
            this.server.WithMetadata(this.message, key, value);

            return this;
        }

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        internal NativeMessageBuilder WithJsonContent(dynamic body) => WithJsonContent(body, this.defaultSettings);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        internal NativeMessageBuilder WithJsonContent(dynamic body, JsonSerializerSettings settings)
        {
            string serialised = JsonConvert.SerializeObject(body, settings);

            this.server.WithContents(this.message, "application/json", serialised, 0);
            return this;
        }

        #endregion Internal Methods
    }
}
