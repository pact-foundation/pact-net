using System.Collections.Generic;

namespace PactNet
{
    /// <summary>
    /// Synchronous plugin request builder for V4 pacts
    /// </summary>
    public interface ISynchronousPluginRequestBuilderV4
    {
        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="description">Provider state description</param>
        /// <returns>Fluent builder</returns>
        ISynchronousPluginRequestBuilderV4 Given(string description);

        /// <summary>
        /// Add a provider state with a parameter to the interaction
        /// </summary>
        /// <param name="description">Provider state description</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        ISynchronousPluginRequestBuilderV4 Given(string description, string name, string value);

        /// <summary>
        /// Set the plugin interaction contents. This completes the interaction and must be the last call.
        /// </summary>
        /// <remarks>
        /// The plugin interprets the contents and splits them into the request and response(s) itself, so
        /// this is called exactly once per interaction. Setting the contents again would replace the request
        /// but append a further response, which is why the builder does not allow a second call.
        /// </remarks>
        /// <param name="contentType">Content type understood by the plugin, e.g. <c>application/grpc</c></param>
        /// <param name="content">A dictionary containing the plugin content.</param>
        void WithContent(string contentType, Dictionary<string, object> content);
    }
}
