using System;

namespace PactNet.Native
{
    /// <summary>
    /// Native mock server context
    /// </summary>
    public class NativePactContext : IPactContext
    {
        private readonly IMockServer server;
        private readonly PactConfig config;

        /// <summary>
        /// URI for the mock server
        /// </summary>
        public Uri MockServerUri { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="NativePactContext"/> class.
        /// </summary>
        /// <param name="server">Mock server</param>
        /// <param name="mockServerUri">Mock server URI</param>
        /// <param name="config">Pact config</param>
        internal NativePactContext(IMockServer server, Uri mockServerUri, PactConfig config)
        {
            this.server = server;
            this.config = config;
            this.MockServerUri = mockServerUri;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            try
            {
                /* TODO: Uncomment this once the new FFI release is available which supports it

                string logs = this.server.MockServerLogs(this.MockServerUri.Port);

                if (!string.IsNullOrWhiteSpace(logs))
                {
                    this.config.WriteLine("Mock server logs:");
                    this.config.WriteLine(string.Empty);
                    this.config.WriteLine(logs);
                }*/

                string errors = this.server.MockServerMismatches(this.MockServerUri.Port);

                if (string.IsNullOrWhiteSpace(errors) || errors == "[]")
                {
                    this.server.WritePactFile(this.MockServerUri.Port, this.config.PactDir, false);
                    return;
                }

                this.config.WriteLine("Verification mismatches:");
                this.config.WriteLine(string.Empty);
                this.config.WriteLine(errors);

                throw new PactFailureException("Pact verification failed. See output for details");
            }
            finally
            {
                this.ReleaseUnmanagedResources();
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Clean up the mock server resources
        /// </summary>
        private void ReleaseUnmanagedResources()
        {
            this.server.CleanupMockServer(this.MockServerUri.Port);
        }

        /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
        ~NativePactContext()
        {
            this.ReleaseUnmanagedResources();
        }
    }
}
