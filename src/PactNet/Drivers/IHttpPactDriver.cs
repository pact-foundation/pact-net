using System;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for synchronous HTTP pacts
    /// </summary>
    internal interface IHttpPactDriver : ICompletedPactDriver, IDisposable
    {
        /// <summary>
        /// Create a new interaction on the current pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>HTTP interaction handle</returns>
        IHttpInteractionDriver NewHttpInteraction(string description);

        /// <summary>
        /// Create the mock server for the current pact
        /// </summary>
        /// <param name="host">Host for the mock server</param>
        /// <param name="port">Port for the mock server, or null to allocate one automatically</param>
        /// <param name="tls">Enable TLS</param>
        /// <returns>Mock server port</returns>
        /// <exception cref="InvalidOperationException">Failed to start mock server</exception>
        IMockServerDriver CreateMockServer(string host, int? port, bool tls);
    }
}
