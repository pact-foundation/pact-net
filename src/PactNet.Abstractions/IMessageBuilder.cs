using System.Collections.Generic;
using Newtonsoft.Json;

namespace PactNet
{
    /// <summary>
    /// Build up a mock message for a v3 message messagePact
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
        /// <returns>Fluent builder</returns>
        IMessageBuilderV3 WithJsonContent(dynamic body);

        /// <summary>
        /// Set message content which is serialised as JSON
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV3 WithJsonContent(dynamic body, JsonSerializerSettings settings);
    }
}
