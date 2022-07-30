using System;
using System.Threading.Tasks;
using PactNet.Drivers;
using PactNet.Exceptions;
using PactNet.Internal;
using PactNet.Interop;
using PactNet.Models;

namespace PactNet
{
    /// <summary>
    /// Pact builder for the native backend
    /// </summary>
    internal class PactBuilder : IPactBuilderV2, IPactBuilderV3
    {
        private readonly ISynchronousHttpDriver driver;
        private readonly PactHandle pact;
        private readonly PactConfig config;
        private readonly int? port;
        private readonly IPAddress host;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactBuilder"/> class.
        /// </summary>
        /// <param name="driver">Interaction driver</param>
        /// <param name="pact">Pact handle</param>
        /// <param name="config">Pact config</param>
        /// <param name="port">Optional port, otherwise one is dynamically allocated</param>
        /// <param name="host">Optional host, otherwise loopback is used</param>
        internal PactBuilder(ISynchronousHttpDriver driver, PactHandle pact, PactConfig config, int? port = null, IPAddress host = IPAddress.Loopback)
        {
            this.driver = driver;
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
        internal RequestBuilder UponReceiving(string description)
        {
            InteractionHandle interaction = this.driver.NewHttpInteraction(this.pact, description);

            var requestBuilder = new RequestBuilder(this.driver, interaction, this.config.DefaultJsonSettings);
            return requestBuilder;
        }

        /// <summary>
        /// Verify the configured interactions
        /// </summary>
        /// <param name="interact">Action to perform the real interactions against the mock driver</param>
        /// <exception cref="PactFailureException">Failed to verify the interactions</exception>
        public void Verify(Action<IConsumerContext> interact)
        {
            Guard.NotNull(interact, nameof(interact));

            Uri uri = this.StartMockServer();

            try
            {
                interact(new ConsumerContext
                {
                    MockServerUri = uri
                });

                this.VerifyInternal(uri);
            }
            finally
            {
                this.PrintLogs(uri);
                this.driver.CleanupMockServer(uri.Port);
            }
        }

        /// <summary>
        /// Verify the configured interactions
        /// </summary>
        /// <param name="interact">Action to perform the real interactions against the mock driver</param>
        /// <exception cref="PactFailureException">Failed to verify the interactions</exception>
        public async Task VerifyAsync(Func<IConsumerContext, Task> interact)
        {
            Guard.NotNull(interact, nameof(interact));

            Uri uri = this.StartMockServer();

            try
            {
                await interact(new ConsumerContext
                {
                    MockServerUri = uri
                });

                this.VerifyInternal(uri);
            }
            finally
            {
                this.PrintLogs(uri);
                this.driver.CleanupMockServer(uri.Port);
            }
        }

        /// <summary>
        /// Start the mock driver
        /// </summary>
        /// <returns>Mock driver URI</returns>
        private Uri StartMockServer()
        {
            string hostIp = this.host switch
            {
                IPAddress.Loopback => "127.0.0.1",
                IPAddress.Any => "0.0.0.0",
                _ => throw new ArgumentOutOfRangeException(nameof(this.host), this.host, "Unsupported IPAddress value")
            };

            string address = $"{hostIp}:{this.port.GetValueOrDefault(0)}";

            // TODO: add TLS support
            int serverPort = this.driver.CreateMockServerForPact(this.pact, address, false);

            var mockServerUrl = $"http://{hostIp}:{serverPort}";
            var uri = new Uri(mockServerUrl);
            return uri;
        }

        /// <summary>
        /// Verify the interactions after the consumer client has been invoked
        /// </summary>
        private void VerifyInternal(Uri uri)
        {
            string errors = this.driver.MockServerMismatches(uri.Port);

            if (string.IsNullOrWhiteSpace(errors) || errors == "[]")
            {
                this.driver.WritePactFile(this.pact, this.config.PactDir, false);
                return;
            }

            this.config.WriteLine(string.Empty);
            this.config.WriteLine("Verification mismatches:");
            this.config.WriteLine(string.Empty);
            this.config.WriteLine(errors);

            throw new PactFailureException("Pact verification failed. See output for details");
        }

        /// <summary>
        /// Print logs to the configured outputs
        /// </summary>
        /// <param name="uri">Mock driver URI</param>
        private void PrintLogs(Uri uri)
        {
            string logs = this.driver.MockServerLogs(uri.Port);

            this.config.WriteLine("Mock driver logs:");
            this.config.WriteLine(string.Empty);
            this.config.WriteLine(logs);
        }
    }
}
