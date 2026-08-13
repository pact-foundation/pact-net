using System.Collections.Generic;
using System.Text.Json;

namespace PactNet
{
    /// <summary>
    /// Build up a mock message for a v3 message message pact
    /// </summary>
    public interface IMessageBuilderV3
    {
        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV3 Given(string providerState);

        /// <summary>
        /// Add a provider state with one or more parameters
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <param name="parameters">Provider state parameters</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV3 Given(string providerState, IDictionary<string, string> parameters);

        /// <summary>
        /// Set the metadata
        /// </summary>
        /// <param name="key">the metadata key</param>
        /// <param name="value">the metadata value</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV3 WithMetadata(string key, string value);

        /// <summary>
        /// Set message content which is serialised as JSON
        /// </summary>
        /// <param name="body">Message body</param>
        /// <returns>Configured message</returns>
        IConfiguredMessageVerifier WithJsonContent(dynamic body);

        /// <summary>
        /// Set message content which is serialised as JSON
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Configured message</returns>
        IConfiguredMessageVerifier WithJsonContent(dynamic body, JsonSerializerOptions settings);
    }

    /// <summary>
    /// Build up a mock message for a v4 message message pact
    /// </summary>
    public interface IMessageBuilderV4
    {
        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV4 Given(string providerState);

        /// <summary>
        /// Add a provider state with one or more parameters
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <param name="parameters">Provider state parameters</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV4 Given(string providerState, IDictionary<string, string> parameters);

        /// <summary>
        /// Set the metadata
        /// </summary>
        /// <param name="key">the metadata key</param>
        /// <param name="value">the metadata value</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV4 WithMetadata(string key, string value);

        /// <summary>
        /// Add a comment with a key-value pair to the interaction
        /// </summary>
        /// <param name="key">the comment key</param>
        /// <param name="value">the comment value</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV4 WithComment(string key, string value);

        /// <summary>
        /// Add a text comment to the interaction text comments array
        /// </summary>
        /// <param name="comment">the text comment</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV4 WithTextComment(string comment);

        /// <summary>
        /// Add an AsyncAPI operation reference to the interaction
        /// </summary>
        /// <param name="operationId">the AsyncAPI operation ID</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>
        /// If called multiple times, only the last operationId will be retained.
        /// For multiple reference types, use WithComment() with a custom JSON structure.
        /// </remarks>
        IMessageBuilderV4 WithAsyncApiReference(string operationId);

        /// <summary>
        /// Set message content which is serialised as JSON
        /// </summary>
        /// <param name="body">Message body</param>
        /// <returns>Configured message</returns>
        IConfiguredMessageVerifier WithJsonContent(dynamic body);

        /// <summary>
        /// Set message content which is serialised as JSON
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Configured message</returns>
        IConfiguredMessageVerifier WithJsonContent(dynamic body, JsonSerializerOptions settings);
    }
}
