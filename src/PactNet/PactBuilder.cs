using System;
using System.Threading.Tasks;
using PactNet.Drivers;
using PactNet.Exceptions;
using PactNet.Internal;
using PactNet.Models;

namespace PactNet
{
    /// <summary>
    /// Pact builder for the native backend
    /// </summary>
    internal class PactBuilder : IPactBuilderV2, IPactBuilderV3, IPactBuilderV4
    {
        private readonly IHttpPactDriver pact;
        private readonly PactConfig config;
        private readonly int? port;
        private readonly IPAddress host;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactBuilder"/> class.
        /// </summary>
        /// <param name="pact">Pact driver</param>
        /// <param name="config">Pact config</param>
        /// <param name="port">Optional port, otherwise one is dynamically allocated</param>
        /// <param name="host">Optional host, otherwise loopback is used</param>
        internal PactBuilder(IHttpPactDriver pact, PactConfig config, int? port = null, IPAddress host = IPAddress.Loopback)
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
        /// Add a new interaction to the pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV4 IPactBuilderV4.UponReceiving(string description)
            => this.UponReceiving(description);

        /// <summary>
        /// Create a new request/response interaction
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Request builder</returns>
        internal RequestBuilder UponReceiving(string description)
        {
            IHttpInteractionDriver interactions = this.pact.NewHttpInteraction(description);

            var requestBuilder = new RequestBuilder(interactions, this.config.DefaultJsonSettings);
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

            using IMockServerDriver mockServer = this.StartMockServer();

            try
            {
                interact(new ConsumerContext
                {
                    MockServerUri = mockServer.Uri
                });

                this.VerifyInternal(mockServer);
            }
            finally
            {
                this.PrintLogs(mockServer);
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

            using IMockServerDriver mockServer = this.StartMockServer();

            try
            {
                await interact(new ConsumerContext
                {
                    MockServerUri = mockServer.Uri
                });

                this.VerifyInternal(mockServer);
            }
            finally
            {
                this.PrintLogs(mockServer);
            }
        }

        /// <summary>
        /// Start the mock driver
        /// </summary>
        /// <returns>Mock driver</returns>
        private IMockServerDriver StartMockServer()
        {
            string hostIp = this.host switch
            {
                IPAddress.Loopback => "127.0.0.1",
                IPAddress.Any => "0.0.0.0",
                _ => throw new ArgumentOutOfRangeException(nameof(this.host), this.host, "Unsupported IPAddress value")
            };

            // TODO: add TLS support
            return this.pact.CreateMockServer(hostIp, this.port, false);
        }

        /// <summary>
        /// Verify the interactions after the consumer client has been invoked
        /// </summary>
        /// <param name="mockServer">Mock server</param>
        private void VerifyInternal(IMockServerDriver mockServer)
        {
            string errors = mockServer.MockServerMismatches();

            if (string.IsNullOrWhiteSpace(errors) || errors == "[]")
            {
                this.pact.WritePactFile(this.config.PactDir);
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
        /// <param name="mockServer">Mock server</param>
        private void PrintLogs(IMockServerDriver mockServer)
        {
            string logs = mockServer.MockServerLogs();

            this.config.WriteLine("Mock driver logs:");
            this.config.WriteLine(string.Empty);
            this.config.WriteLine(logs);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.pact.Dispose();
        }
    }
}
