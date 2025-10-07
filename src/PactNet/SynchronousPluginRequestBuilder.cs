using System.Collections.Generic;
using PactNet.Drivers.Plugins;

namespace PactNet;

internal class SynchronousPluginRequestBuilder(IPluginInteractionDriver interactionDriver)
    : ISynchronousPluginRequestBuilderV4
{

    /// <summary>
    /// Add a provider state
    /// </summary>
    /// <param name="description">Provider state description</param>
    /// <returns>Fluent builder</returns>
    public ISynchronousPluginRequestBuilderV4 Given(string description)
    {
        interactionDriver.Given(description);
        return this;
    }

    /// <summary>
    /// Add a provider state with a parameter to the interaction
    /// </summary>
    /// <param name="description">Provider state description</param>
    /// <param name="name">Parameter name</param>
    /// <param name="value">Parameter value</param>
    public ISynchronousPluginRequestBuilderV4 Given(string description, string name, string value)
    {
        interactionDriver.GivenWithParam(description, name, value);
        return this;
    }

    /// <summary>
    /// Add plugin interaction content
    /// </summary>
    /// <param name="contentType">Content type</param>
    /// <param name="content">A dictionary containing the plugin content.</param>
    public ISynchronousPluginRequestBuilderV4 WithContent(string contentType, Dictionary<string, object> content)
    {
        interactionDriver.WithContent(contentType, content);
        return this;
    }
}
