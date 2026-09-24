using System.Collections.Generic;
using System.Text.Json;

namespace PactNet
{
    /// <summary>
    /// Build up a mock synchronous (request/response) message for a v4 message pact
    /// </summary>
    public interface ISyncMessageBuilderV4
    {
        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        ISyncMessageBuilderV4 Given(string providerState);

        /// <summary>
        /// Add a provider state with one or more parameters
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <param name="parameters">Provider state parameters</param>
        /// <returns>Fluent builder</returns>
        ISyncMessageBuilderV4 Given(string providerState, IDictionary<string, string> parameters);

        /// <summary>
        /// Set the request metadata
        /// </summary>
        /// <param name="key">the metadata key</param>
        /// <param name="value">the metadata value</param>
        /// <returns>Fluent builder</returns>
        ISyncMessageBuilderV4 WithRequestMetadata(string key, string value);

        /// <summary>
        /// Set the response metadata
        /// </summary>
        /// <param name="key">the metadata key</param>
        /// <param name="value">the metadata value</param>
        /// <returns>Fluent builder</returns>
        ISyncMessageBuilderV4 WithResponseMetadata(string key, string value);

        /// <summary>
        /// Set the request content which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        ISyncMessageBuilderV4 WithRequestJsonContent(dynamic body);

        /// <summary>
        /// Set the request content which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        ISyncMessageBuilderV4 WithRequestJsonContent(dynamic body, JsonSerializerOptions settings);

        /// <summary>
        /// Set the response content which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <returns>Configured message</returns>
        IConfiguredSyncMessageVerifier WithResponseJsonContent(dynamic body);

        /// <summary>
        /// Set the response content which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Configured message</returns>
        IConfiguredSyncMessageVerifier WithResponseJsonContent(dynamic body, JsonSerializerOptions settings);
    }
}
