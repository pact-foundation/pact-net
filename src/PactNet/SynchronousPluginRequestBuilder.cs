using System.Collections.Generic;
using PactNet.Drivers.Plugins;

namespace PactNet
{
    /// <summary>
    /// Synchronous plugin request builder
    /// </summary>
    internal class SynchronousPluginRequestBuilder : ISynchronousPluginRequestBuilderV4
    {
        private readonly IPluginInteractionDriver interactionDriver;

        /// <summary>
        /// Initialises a new instance of the <see cref="SynchronousPluginRequestBuilder"/> class.
        /// </summary>
        /// <param name="interactionDriver">Interaction driver</param>
        public SynchronousPluginRequestBuilder(IPluginInteractionDriver interactionDriver)
        {
            this.interactionDriver = interactionDriver;
        }

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="description">Provider state description</param>
        /// <returns>Fluent builder</returns>
        public ISynchronousPluginRequestBuilderV4 Given(string description)
        {
            this.interactionDriver.Given(description);
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
            this.interactionDriver.GivenWithParam(description, name, value);
            return this;
        }

        /// <summary>
        /// Set the plugin interaction contents. This completes the interaction and must be the last call.
        /// </summary>
        /// <param name="contentType">Content type understood by the plugin</param>
        /// <param name="content">A dictionary containing the plugin content.</param>
        public void WithContent(string contentType, Dictionary<string, object> content)
        {
            this.interactionDriver.WithContent(contentType, content);
        }
    }
}
