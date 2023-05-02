using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Drivers;
using PactNet.Interop;

namespace PactNet
{
    /// <summary>
    /// Mock request message builder
    /// </summary>
    internal class MessageBuilder : IMessageBuilderV3, IMessageBuilderV4
    {
        private readonly IMessageInteractionDriver driver;
        private readonly PactConfig config;
        private readonly PactSpecification version;

        /// <summary>
        /// Initialises a new instance of the <see cref="MessagePactBuilder"/> class.
        /// </summary>
        /// <param name="server">Interaction driver</param>
        /// <param name="config">Pact config</param>
        /// <param name="version">Pact specification version</param>
        internal MessageBuilder(IMessageInteractionDriver server, PactConfig config, PactSpecification version)
        {
            this.driver = server ?? throw new ArgumentNullException(nameof(server));
            this.config = config;
            this.version = version;
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
        IConfiguredMessageVerifier IMessageBuilderV3.WithJsonContent(dynamic content)
            => WithJsonContent(content);

        /// <inheritdoc cref="IMessageBuilderV3"/>
        IConfiguredMessageVerifier IMessageBuilderV3.WithJsonContent(dynamic content, JsonSerializerSettings settings)
            => WithJsonContent(content, settings);

        #endregion

        #region IMessagePactBuilderV4 explicit implementation

        /// <inheritdoc cref="IMessageBuilderV4"/>
        IMessageBuilderV4 IMessageBuilderV4.Given(string providerState)
            => Given(providerState);

        /// <inheritdoc cref="IMessageBuilderV4"/>
        IMessageBuilderV4 IMessageBuilderV4.Given(string providerState, IDictionary<string, string> parameters)
            => Given(providerState, parameters);

        /// <inheritdoc cref="IMessageBuilderV4"/>
        IMessageBuilderV4 IMessageBuilderV4.WithMetadata(string key, string value)
            => WithMetadata(key, value);

        /// <inheritdoc cref="IMessageBuilderV4"/>
        IConfiguredMessageVerifier IMessageBuilderV4.WithJsonContent(dynamic content)
            => WithJsonContent(content);

        /// <inheritdoc cref="IMessageBuilderV4"/>
        IConfiguredMessageVerifier IMessageBuilderV4.WithJsonContent(dynamic content, JsonSerializerSettings settings)
            => WithJsonContent(content, settings);

        #endregion

        #region Internal Methods

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        internal MessageBuilder Given(string providerState)
        {
            this.driver.Given(providerState);

            return this;
        }

        /// <summary>
        /// Add a provider state with one or more parameters
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <param name="parameters">Provider state parameters</param>
        /// <returns>Fluent builder</returns>
        internal MessageBuilder Given(string providerState, IDictionary<string, string> parameters)
        {
            foreach (var param in parameters)
            {
                this.driver.GivenWithParam(providerState, param.Key, param.Value);
            }

            return this;
        }

        /// <summary>
        /// Set the metadata
        /// </summary>
        /// <param name="key">key of the metadata</param>
        /// <param name="value">value of the metadata</param>
        /// <returns>Fluent builder</returns>
        internal MessageBuilder WithMetadata(string key, string value)
        {
            this.driver.WithMetadata(key, value);

            return this;
        }

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        internal ConfiguredMessageVerifier WithJsonContent(dynamic body) => WithJsonContent(body, this.config.DefaultJsonSettings);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        internal ConfiguredMessageVerifier WithJsonContent(dynamic body, JsonSerializerSettings settings)
        {
            string serialised = JsonConvert.SerializeObject(body, settings);

            this.driver.WithContents("application/json", serialised, 0);

            return new ConfiguredMessageVerifier(this.driver, this.config, this.version);
        }

        #endregion Internal Methods
    }
}
