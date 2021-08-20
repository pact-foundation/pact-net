using System;

using System.Threading.Tasks;

using PactNet.Exceptions;
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
        /// Add a new interaction to the messagePact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IPactBuilderV2.UponReceiving(string description)
            => UponReceiving(description);

        /// <summary>
        /// Add a new interaction to the messagePact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IPactBuilderV3.UponReceiving(string description)
            => UponReceiving(description);

        /// <summary>
        /// Create a new request/response interaction
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Request builder</returns>
        internal NativeRequestBuilder UponReceiving(string description)
        {
            InteractionHandle interaction = server.NewInteraction(pact, description);

            var requestBuilder = new NativeRequestBuilder(server, interaction, config.DefaultJsonSettings);
            return requestBuilder;
        }

        /// <summary>
        /// Verify the configured interactions
        /// </summary>
        /// <param name="interact">Action to perform the real interactions against the mock server</param>
        /// <exception cref="PactFailureException">Failed to verify the interactions</exception>
        public void Verify(Action<IConsumerContext> interact)
        {
            if (interact == null)
            {
                throw new ArgumentNullException(nameof(interact));
            }

            Uri uri = StartMockServer();

            try
            {
                interact(new NativeConsumerContext
                {
                    MockServerUri = uri
                });

                VerifyInternal(uri);
            }
            finally
            {
                server.CleanupMockServer(uri.Port);
            }
        }

        /// <summary>
        /// Verify the configured interactions
        /// </summary>
        /// <param name="interact">Action to perform the real interactions against the mock server</param>
        /// <exception cref="PactFailureException">Failed to verify the interactions</exception>
        public async Task VerifyAsync(Func<IConsumerContext, Task> interact)
        {
            if (interact == null)
            {
                throw new ArgumentNullException(nameof(interact));
            }

            Uri uri = StartMockServer();

            try
            {
                await interact(new NativeConsumerContext
                {
                    MockServerUri = uri
                });

                VerifyInternal(uri);
            }
            finally
            {
                server.CleanupMockServer(uri.Port);
            }
        }

        /// <summary>
        /// Start the mock server
        /// </summary>
        /// <returns>Mock server URI</returns>
        private Uri StartMockServer()
        {
            string hostIp = host switch
            {
                IPAddress.Loopback => "127.0.0.1",
                IPAddress.Any => "0.0.0.0",
                _ => throw new ArgumentOutOfRangeException(nameof(host), host, "Unsupported IPAddress value")
            };

            string address = $"{hostIp}:{port.GetValueOrDefault(0)}";

            // TODO: add TLS support
            int serverPort = server.CreateMockServerForPact(pact, address, false);

            var uri = new Uri($"http://{host}:{serverPort}");
            return uri;
        }

        /// <summary>
        /// Verify the interactions after the consumer client has been invoked
        /// </summary>
        private void VerifyInternal(Uri uri)
        {
            string errors = server.MockServerMismatches(uri.Port);

            if (string.IsNullOrWhiteSpace(errors) || errors == "[]")
            {
                server.WritePactFile(uri.Port, config.PactDir, false);
                return;
            }

            config.WriteLine("Verification mismatches:");
            config.WriteLine(string.Empty);
            config.WriteLine(errors);

            throw new PactFailureException("Pact verification failed. See output for details");
        }
    }
}
