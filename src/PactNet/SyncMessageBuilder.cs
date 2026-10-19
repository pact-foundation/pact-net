using System;
using System.Collections.Generic;
using System.Text.Json;
using PactNet.Drivers;
using PactNet.Interop;

namespace PactNet
{
    /// <summary>
    /// Mock synchronous (request/response) message builder
    /// </summary>
    internal class SyncMessageBuilder : ISyncMessageBuilderV4
    {
        private readonly ISyncMessageInteractionDriver driver;
        private readonly PactConfig config;
        private readonly PactSpecification version;

        // pactffi_with_metadata only applies response metadata to responses that already exist, and the response is
        // only created by the terminal WithResponseJsonContent call, so response metadata is applied after it
        private readonly List<KeyValuePair<string, string>> responseMetadata = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Initialises a new instance of the <see cref="SyncMessageBuilder"/> class.
        /// </summary>
        /// <param name="driver">Interaction driver</param>
        /// <param name="config">Pact config</param>
        /// <param name="version">Pact specification version</param>
        internal SyncMessageBuilder(ISyncMessageInteractionDriver driver, PactConfig config, PactSpecification version)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver));
            this.config = config;
            this.version = version;
        }

        #region ISyncMessageBuilderV4 explicit implementation

        /// <inheritdoc cref="ISyncMessageBuilderV4"/>
        ISyncMessageBuilderV4 ISyncMessageBuilderV4.Given(string providerState)
            => Given(providerState);

        /// <inheritdoc cref="ISyncMessageBuilderV4"/>
        ISyncMessageBuilderV4 ISyncMessageBuilderV4.Given(string providerState, IDictionary<string, string> parameters)
            => Given(providerState, parameters);

        /// <inheritdoc cref="ISyncMessageBuilderV4"/>
        ISyncMessageBuilderV4 ISyncMessageBuilderV4.WithReference(string group, string name, string value)
            => WithReference(group, name, value);

        /// <inheritdoc cref="ISyncMessageBuilderV4"/>
        ISyncMessageBuilderV4 ISyncMessageBuilderV4.WithRequestMetadata(string key, dynamic value)
            => WithRequestMetadata(key, value);

        /// <inheritdoc cref="ISyncMessageBuilderV4"/>
        ISyncMessageBuilderV4 ISyncMessageBuilderV4.WithResponseMetadata(string key, dynamic value)
            => WithResponseMetadata(key, value);

        /// <inheritdoc cref="ISyncMessageBuilderV4"/>
        ISyncMessageBuilderV4 ISyncMessageBuilderV4.WithRequestJsonContent(dynamic body)
            => WithRequestJsonContent(body);

        /// <inheritdoc cref="ISyncMessageBuilderV4"/>
        ISyncMessageBuilderV4 ISyncMessageBuilderV4.WithRequestJsonContent(dynamic body, JsonSerializerOptions settings)
            => WithRequestJsonContent(body, settings);

        /// <inheritdoc cref="ISyncMessageBuilderV4"/>
        IConfiguredSyncMessageVerifier ISyncMessageBuilderV4.WithResponseJsonContent(dynamic body)
            => WithResponseJsonContent(body);

        /// <inheritdoc cref="ISyncMessageBuilderV4"/>
        IConfiguredSyncMessageVerifier ISyncMessageBuilderV4.WithResponseJsonContent(dynamic body, JsonSerializerOptions settings)
            => WithResponseJsonContent(body, settings);

        #endregion

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        internal SyncMessageBuilder Given(string providerState)
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
        internal SyncMessageBuilder Given(string providerState, IDictionary<string, string> parameters)
        {
            foreach (var param in parameters)
            {
                this.driver.GivenWithParam(providerState, param.Key, param.Value);
            }

            return this;
        }

        /// <summary>
        /// Add a reference to an external resource, e.g. an AsyncAPI operation
        /// </summary>
        /// <param name="group">Reference group</param>
        /// <param name="name">Reference name</param>
        /// <param name="value">Reference value</param>
        /// <returns>Fluent builder</returns>
        internal SyncMessageBuilder WithReference(string group, string name, string value)
        {
            this.driver.AddReference(group, name, value);

            return this;
        }

        /// <summary>
        /// Set a request metadata value, which is serialised as JSON
        /// </summary>
        /// <param name="key">key of the metadata</param>
        /// <param name="value">value of the metadata</param>
        /// <returns>Fluent builder</returns>
        internal SyncMessageBuilder WithRequestMetadata(string key, dynamic value)
        {
            string serialised = JsonSerializer.Serialize(value, this.config.DefaultJsonSettings);

            this.driver.WithRequestMetadata(key, serialised);

            return this;
        }

        /// <summary>
        /// Set a response metadata value, which is serialised as JSON
        /// </summary>
        /// <param name="key">key of the metadata</param>
        /// <param name="value">value of the metadata</param>
        /// <returns>Fluent builder</returns>
        internal SyncMessageBuilder WithResponseMetadata(string key, dynamic value)
        {
            string serialised = JsonSerializer.Serialize(value, this.config.DefaultJsonSettings);

            this.responseMetadata.Add(new KeyValuePair<string, string>(key, serialised));

            return this;
        }

        /// <summary>
        /// Set a request body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        internal SyncMessageBuilder WithRequestJsonContent(dynamic body) => WithRequestJsonContent(body, this.config.DefaultJsonSettings);

        /// <summary>
        /// Set a request body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        internal SyncMessageBuilder WithRequestJsonContent(dynamic body, JsonSerializerOptions settings)
        {
            string serialised = JsonSerializer.Serialize(body, settings);

            this.driver.WithRequestContents("application/json", serialised);

            return this;
        }

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <returns>Configured message</returns>
        internal ConfiguredSyncMessageVerifier WithResponseJsonContent(dynamic body) => WithResponseJsonContent(body, this.config.DefaultJsonSettings);

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Configured message</returns>
        internal ConfiguredSyncMessageVerifier WithResponseJsonContent(dynamic body, JsonSerializerOptions settings)
        {
            string serialised = JsonSerializer.Serialize(body, settings);

            this.driver.WithResponseContents("application/json", serialised);

            foreach (KeyValuePair<string, string> metadata in this.responseMetadata)
            {
                this.driver.WithResponseMetadata(metadata.Key, metadata.Value);
            }

            return new ConfiguredSyncMessageVerifier(this.driver, this.config, this.version);
        }
    }
}
