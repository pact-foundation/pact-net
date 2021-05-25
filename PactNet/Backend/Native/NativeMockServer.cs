using System;
using PactNet.Models;

namespace PactNet.Backend.Native
{
    /// <summary>
    /// Mock server
    /// </summary>
    public sealed class NativeMockServer : IDisposable, IMockServer
    {
        private readonly string consumer;
        private readonly string provider;
        private readonly PactConfig config;
        private readonly int? port;
        private readonly IPAddress host;

        private int serverPort;
        private PactHandle pact;

        /// <summary>
        /// Static constructor for <see cref="NativeMockServer"/>
        /// </summary>
        static NativeMockServer()
        {
            MockServerInterop.Init(null);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="NativeMockServer"/> class.
        /// </summary>
        /// <param name="consumer">Consumer name</param>
        /// <param name="provider">Provider name</param>
        /// <param name="config">Pact config</param>
        /// <param name="port">Optional port, otherwise one is dynamically allocated</param>
        /// <param name="host">Optional host, otherwise loopback is used</param>
        internal NativeMockServer(string consumer, string provider, PactConfig config, int? port = null, IPAddress host = IPAddress.Loopback)
        {
            this.consumer = consumer;
            this.provider = provider;
            this.config = config;
            this.port = port;
            this.host = host;
        }

        /// <summary>
        /// Start a new pact
        /// </summary>
        /// <returns>Interaction builder for the new pact</returns>
        public IInteractionBuilder CreatePact()
        {
            this.pact = MockServerInterop.NewPact(this.consumer, this.provider);

            PactSpecification specification = this.config.SpecificationVersion switch
            {
                "1.0.0" => PactSpecification.V1,
                "1.1.0" => PactSpecification.V1_1,
                "2.0.0" => PactSpecification.V2,
                "3.0.0" => PactSpecification.V3,
                "4.0.0" => PactSpecification.V4,
                _ => PactSpecification.Unknown
            };

            MockServerInterop.WithSpecification(this.pact, specification);

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

            return new NativeInteractionBuilder(this.pact, this.serverPort, this.config);
        }

        /// <summary>
        /// Write the pact file and shut down the server
        /// </summary>
        public void WritePactFile()
        {
            int result = MockServerInterop.WritePactFile(this.serverPort, this.config.PactDir, this.config.Overwrite);

            switch (result)
            {
                case 1: throw new InvalidOperationException("The pact reference library panicked");
                case 2: throw new InvalidOperationException("The pact file was not able to be written");
                case 3: throw new InvalidOperationException("A mock server with the provided port was not found");
            };
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
        ~NativeMockServer()
        {
            this.ReleaseUnmanagedResources();
        }
    }
}