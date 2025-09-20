using System;
using PactNet.Interop;

namespace PactNet.Drivers.Plugins
{
    /// <summary>
    /// Driver for plugin-based pacts
    /// </summary>
    internal class PluginPactDriver : AbstractPactDriver, IPluginPactDriver
    {
        private readonly PactHandle pact;

        /// <summary>
        /// Initialize a new instance of the <see cref="PluginPactDriver"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        internal PluginPactDriver(PactHandle pact) : base(pact)
        {
            this.pact = pact;
        }

        /// <summary>
        /// Create a new sync interaction on the current pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Interaction driver</returns>
        public IPluginInteractionDriver NewSyncInteraction(string description)
        {
            InteractionHandle interaction = NativeInterop.NewSyncMessageInteraction(this.pact, description);
            return new PluginInteractionDriver(this.pact, interaction);
        }

        /// <summary>
        /// Create the mock server for the current pact
        /// </summary>
        /// <param name="host">Host for the mock server</param>
        /// <param name="port">Port for the mock server, or null to allocate one automatically</param>
        /// <param name="transport">Transport - e.g. http, https, grpc</param>
        /// <param name="transportConfig">Transport config string</param>
        /// <returns>Mock server port</returns>
        /// <exception cref="InvalidOperationException">Failed to start mock server</exception>
        public IMockServerDriver CreateMockServer(string host, int? port, string transport, string transportConfig)
        {
            int result = NativeInterop.CreateMockServerForTransport(this.pact, host, (ushort)port.GetValueOrDefault(0), transport, transportConfig);

            if (result > 0)
            {
                return new MockServerDriver(host, result, false);
            }

            throw result switch
            {
                -1 => new InvalidOperationException("Invalid handle when starting mock server"),
                -2 => new InvalidOperationException("The transport config is not valid JSON"),
                -3 => new InvalidOperationException("Unable to start mock server"),
                -4 => new InvalidOperationException("The pact reference library panicked"),
                -5 => new InvalidOperationException("The IPAddress is invalid"),
                _ => new InvalidOperationException($"Unknown mock server error: {result}")
            };
        }
    }
}
