using System.Collections.Generic;

namespace PactNet
{
    /// <summary>
    /// Build up a mock message for a v3 message pact
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
        /// Set the content
        /// </summary>
        /// <param name="content">Dynamic content</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV3 WithContent(dynamic content);
    }
}
