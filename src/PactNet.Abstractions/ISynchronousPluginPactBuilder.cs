using System;

namespace PactNet
{
    /// <summary>
    /// Synchronous plugin-based pact builder
    /// </summary>
    public interface ISynchronousPluginPactBuilderV4 : IPactBuilder, IDisposable
    {
        /// <summary>
        /// Add a new interaction to the pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        ISynchronousPluginRequestBuilderV4 UponReceiving(string description);
    }
}
