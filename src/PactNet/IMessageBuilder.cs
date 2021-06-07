using System;
using Newtonsoft.Json.Bson;

namespace PactNet
{
    /// <summary>
    /// Build up a mock message for a v3 pact
    /// </summary>
    public interface IPactMessageBuilderV3
    {
        /// <summary>
        /// Add a new message to the pact
        /// </summary>
        /// <param name="description">Message description</param>
        /// <returns>Fluent builder</returns>
        IPactMessageBuilderV3 ExpectsToReceive(string description);

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        IPactMessageBuilderV3 Given(string providerState);

        /// <summary>
        /// Set the metadata
        /// </summary>
        /// <param name="metadata">Dynamic metadata</param>
        /// <returns>Fluent builder</returns>
        IPactMessageBuilderV3 WithMetadata(string key, string value);

        /// <summary>
        /// Set the content
        /// </summary>
        /// <param name="content">Dynamic content</param>
        /// <returns>Fluent builder</returns>
        IPactMessageBuilderV3 WithContent(dynamic content);

        /// <summary>
        /// Build the pact file
        /// </summary>
        void Build();

        /// <summary>
        /// Verify a message is exe
        /// </summary>
        /// <param name="action"></param>
        void Verify(Action action);
    }
}
