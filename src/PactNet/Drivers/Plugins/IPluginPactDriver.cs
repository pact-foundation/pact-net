using System;

namespace PactNet.Drivers.Plugins
{
    /// <summary>
    /// Driver for plugin-based pacts
    /// </summary>
    internal interface IPluginPactDriver : ICompletedPactDriver
    {
        /// <summary>
        /// Create a new sync interaction on the current pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Interaction driver</returns>
        IPluginInteractionDriver NewSyncInteraction(string description);

        /// <summary>
        /// Create the mock server for the current pact
        /// </summary>
        /// <param name="host">Host for the mock server</param>
        /// <param name="port">Port for the mock server, or null to allocate one automatically</param>
        /// <param name="transport">Transport - e.g. http, https, grpc</param>
        /// <param name="transportConfig">Transport config string</param>
        /// <returns>Mock server port</returns>
        /// <exception cref="InvalidOperationException">Failed to start mock server</exception>
        IMockServerDriver CreateMockServer(string host, int? port, string transport, string transportConfig);
    }
}
