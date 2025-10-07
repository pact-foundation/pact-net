using System.Collections.Generic;

namespace PactNet;

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
    /// Add plugin interaction content
    /// </summary>
    /// <param name="contentType">Content type</param>
    /// <param name="content">A dictionary containing the plugin content.</param>
    ISynchronousPluginRequestBuilderV4 WithContent(string contentType, Dictionary<string, object> content);
}
