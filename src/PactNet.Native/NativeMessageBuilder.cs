using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace PactNet.Native
{
    /// <summary>
    /// Mock request message builder
    /// </summary>
    public class NativeMessageBuilder : IMessageBuilderV3
    {
        private readonly IMessageMockServer server;
        private readonly MessageHandle message;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativePactMessageBuilder"/> class.
        /// </summary>
        /// <param name="server">Mock server</param>
        /// <param name="pact">the pact handle</param>
        internal NativeMessageBuilder(IMessageMockServer server, MessageHandle message)
        {
            this.server = server ?? throw new ArgumentNullException(nameof(server));
            this.message = message;
        }

        #region IPactMessageBuilderV3 explicit implementation

        /// <inheritdoc cref="IMessageBuilderV3"/>
        IMessageBuilderV3 IMessageBuilderV3.Given(string providerState)
            => Given(providerState);

        /// <inheritdoc cref="IMessageBuilderV3"/>
        IMessageBuilderV3 IMessageBuilderV3.Given(string providerState, IDictionary<string, string> parameters)
            => Given(providerState, parameters);

        /// <inheritdoc cref="IMessageBuilderV3"/>
        IMessageBuilderV3 IMessageBuilderV3.WithMetadata(string key, string value)
            => WithMetadata(key, value);

        /// <inheritdoc cref="IMessageBuilderV3"/>
        IMessageBuilderV3 IMessageBuilderV3.WithContent(dynamic content)
            => WithContent(content);

        #endregion

        #region Internal Methods

        /// <summary>
        /// Add a new message to the pact
        /// </summary>
        /// <param name="description">Message description</param>
        /// <returns>Fluent builder</returns>
        internal IMessageBuilderV3 ExpectsToReceive(string description)
        {
            server.ExpectsToReceive(message, description);

            return this;
        }

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        internal IMessageBuilderV3 Given(string providerState)
        {
            server.Given(message, providerState);

            return this;
        }

        /// <summary>
        /// Add a provider state with one or more parameters
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <param name="parameters">Provider state parameters</param>
        /// <returns>Fluent builder</returns>
        internal IMessageBuilderV3 Given(string providerState, IDictionary<string, string> parameters)
        {
            foreach (var param in parameters)
            {
                server.GivenWithParam(message, providerState, param.Key, param.Value);
            }

            return this;
        }

        /// <summary>
        /// Set the metadata
        /// </summary>
        /// <param name="key">key of the metadata</param>
        /// <param name="value">value of the metadata</param>
        /// <returns>Fluent builder</returns>
        internal IMessageBuilderV3 WithMetadata(string key, string value)
        {
            server.WithMetadata(message, key, value);

            return this;
        }

        /// <summary>
        /// Set the content
        /// </summary>
        /// <param name="content">Dynamic content</param>
        /// <returns>Fluent builder</returns>
        internal IMessageBuilderV3 WithContent(dynamic content)
        {
            server.WithContents(message, "application/json", JsonConvert.SerializeObject(content), 100);

            return this;
        }

        #endregion Internal Methods
    }
}
