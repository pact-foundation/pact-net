using System;
using PactNet.Models;

namespace PactNet.Native
{
    /// <summary>
    /// Pact builder for the native backend
    /// </summary>
    public class NativePactBuilder : IPactBuilderV2, IPactBuilderV3
    {
        private readonly IMockServer server;
        private readonly PactHandle pact;
        private readonly PactConfig config;
        private readonly int? port;
        private readonly IPAddress host;

        private int serverPort;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativePactBuilder"/> class.
        /// </summary>
        /// <param name="server">Mock server</param>
        /// <param name="pact">Pact handle</param>
        /// <param name="config">Pact config</param>
        /// <param name="port">Optional port, otherwise one is dynamically allocated</param>
        /// <param name="host">Optional host, otherwise loopback is used</param>
        internal NativePactBuilder(IMockServer server, PactHandle pact, PactConfig config, int? port = null, IPAddress host = IPAddress.Loopback)
        {
            this.server = server;
            this.pact = pact;
            this.config = config;
            this.port = port;
            this.host = host;
        }

        /// <summary>
        /// Add a new interaction to the pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IPactBuilderV2.UponReceiving(string description)
            => this.UponReceiving(description);

        /// <summary>
        /// Add a new interaction to the pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IPactBuilderV3.UponReceiving(string description)
            => this.UponReceiving(description);

        /// <summary>
        /// Create a new request/response interaction
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Request builder</returns>
        internal NativeRequestBuilder UponReceiving(string description)
        {
            InteractionHandle interaction = this.server.NewInteraction(this.pact, description);

            var requestBuilder = new NativeRequestBuilder(this.server, interaction, this.config.DefaultJsonSettings);
            return requestBuilder;
        }

        /// <summary>
        /// Finalise the pact
        /// </summary>
        /// <returns>Pact context in which to run interactions</returns>
        public IPactContext Build()
        {
            string hostIp = this.host switch
            {
                IPAddress.Loopback => "127.0.0.1",
                IPAddress.Any => "0.0.0.0",
                _ => throw new ArgumentOutOfRangeException(nameof(this.host), this.host, "Unsupported IPAddress value")
            };

            string address = $"{hostIp}:{this.port.GetValueOrDefault(0)}";

            // TODO: add TLS support
            this.serverPort = this.server.CreateMockServerForPact(this.pact, address, false);

            Uri uri = new Uri($"http://{this.host}:{this.serverPort}");
            return new NativePactContext(this.server, uri, this.config);
        }
    }
}
