using System;
using PactNet.Models;

namespace PactNet.Native
{
    /// <summary>
    /// Pact builder for the native backend
    /// </summary>
    public class NativePactBuilder : IPactBuilder
    {
        private readonly PactHandle pact;
        private readonly PactConfig config;
        private readonly int? port;
        private readonly IPAddress host;

        private int serverPort;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativePactBuilder"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        /// <param name="config">Pact config</param>
        /// <param name="port">Optional port, otherwise one is dynamically allocated</param>
        /// <param name="host">Optional host, otherwise loopback is used</param>
        internal NativePactBuilder(PactHandle pact, PactConfig config, int? port = null, IPAddress host = IPAddress.Loopback)
        {
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
        public IRequestBuilder UponReceiving(string description)
        {
            InteractionHandle interaction = MockServerInterop.NewInteraction(this.pact, description);

            var requestBuilder = new NativeRequestBuilder(interaction, this.config.DefaultJsonSettings);
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
            int result = MockServerInterop.CreateMockServerForPact(this.pact, address, false);

            this.serverPort = result switch
            {
                -1 => throw new InvalidOperationException("Invalid handle when starting mock server"),
                -3 => throw new InvalidOperationException("Unable to start mock server"),
                -4 => throw new InvalidOperationException("The pact reference library panicked"),
                -5 => throw new InvalidOperationException("The IPAddress is invalid"),
                -6 => throw new InvalidOperationException("Could not create the TLS configuration with the self-signed certificate"),
                _ => result
            };

            Uri uri = new Uri($"http://{this.host}:{this.serverPort}");
            return new NativePactContext(uri, this.config);
        }

        /// <summary>
        /// Clean up the mock server resources
        /// </summary>
        private void ReleaseUnmanagedResources()
        {
            if (this.serverPort > 0)
            {
                MockServerInterop.CleanupMockServer(this.serverPort);
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            this.ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
        ~NativePactBuilder()
        {
            this.ReleaseUnmanagedResources();
        }
    }
}