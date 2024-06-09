using System;
using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for synchronous HTTP pacts
    /// </summary>
    internal class HttpPactDriver : AbstractPactDriver, IHttpPactDriver
    {
        private readonly PactHandle pact;

        /// <summary>
        /// Initialises a new instance of the <see cref="HttpPactDriver"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        internal HttpPactDriver(PactHandle pact) : base(pact)
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
            InteractionHandle interaction = NativeInterop.NewInteraction(this.pact, description);
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
        public IMockServerDriver CreateMockServer(string host, int? port, bool tls)
        {
            int result = NativeInterop.CreateMockServerForTransport(this.pact, host, (ushort)port.GetValueOrDefault(0), "http", null);

            if (result > 0)
            {
                return new MockServerDriver(host, result, tls);
            }

            throw result switch
            {
                -1 => new InvalidOperationException("Invalid handle when starting mock server"),
                -3 => new InvalidOperationException("Unable to start mock server"),
                -4 => new InvalidOperationException("The pact reference library panicked"),
                -5 => new InvalidOperationException("The IPAddress is invalid"),
                -6 => new InvalidOperationException("Could not create the TLS configuration with the self-signed certificate"),
                _ => new InvalidOperationException($"Unknown mock server error: {result}")
            };
        }
    }
}
