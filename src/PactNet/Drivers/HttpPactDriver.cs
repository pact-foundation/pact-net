using System;
using PactNet.Interop;
using PactNet.Interop.Drivers;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for synchronous HTTP pacts
    /// </summary>
    internal class HttpPactDriver :  IHttpPactDriver
    {
        private readonly PactHandle pact;

        /// <summary>
        /// Initialises a new instance of the <see cref="HttpPactDriver"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        internal HttpPactDriver(PactHandle pact)
        {
            this.pact = pact;
        }

        /// <summary>
        /// Create a new interaction on the current pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>HTTP interaction handle</returns>
        public IHttpInteractionDriver NewHttpInteraction(string description)
        {
            InteractionHandle interaction = HttpInterop.NewInteraction(this.pact, description);
            return new HttpInteractionDriver(this.pact, interaction);
        }

        /// <summary>
        /// Create the mock server for the current pact
        /// </summary>
        /// <param name="host">Host for the mock server</param>
        /// <param name="port">Port for the mock server, or null to allocate one automatically</param>
        /// <param name="tls">Enable TLS</param>
        /// <returns>Mock server port</returns>
        /// <exception cref="InvalidOperationException">Failed to start mock server</exception>
        public IMockServerDriver CreateMockServer(string host, int? port, bool tls) => this.pact.CreateMockServer(host, port, "http", tls);

        public void WritePactFile(string directory) => PactFileWriter.WritePactFile(this.pact,  directory);
    }
}
