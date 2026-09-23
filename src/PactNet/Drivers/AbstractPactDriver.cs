using System;
using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Abstract pact driver, agnostic of interaction style
    /// </summary>
    internal abstract class AbstractPactDriver : ICompletedPactDriver
    {
        protected readonly PactHandle pact;

        /// <summary>
        /// Initialises a new instance of the <see cref="AbstractPactDriver"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        protected AbstractPactDriver(PactHandle pact)
        {
            this.pact = pact;
        }

        /// <summary>
        /// Write the pact file to disk
        /// </summary>
        /// <param name="directory">Directory of the pact file</param>
        /// <returns>Status code</returns>
        /// <exception cref="InvalidOperationException">Failed to write pact file</exception>
        public void WritePactFile(string directory)
        {
            int result = NativeInterop.WritePactFile(this.pact, directory, false);
            ThrowExceptionOnWritePactFileFailure(result);
        }

        /// <summary>
        /// Write the pact file to disk
        /// </summary>
        /// <param name="port">Port of the mock server</param>
        /// <param name="directory">Directory of the pact file</param>
        public void WritePactFile(int port, string directory)
        {
            int result = NativeInterop.WritePactFileForPort(port, directory, false);
            ThrowExceptionOnWritePactFileFailure(result);
        }

        private static void ThrowExceptionOnWritePactFileFailure(int result)
        {
            if (result != 0)
            {
                throw result switch
                {
                    1 => new InvalidOperationException("The pact reference library panicked"),
                    2 => new InvalidOperationException("The pact file could not be written"),
                    3 => new InvalidOperationException("A mock server with the provided port was not found"),
                    _ => new InvalidOperationException($"Unknown error from backend: {result}")
                };
            }
        }

        /// <summary>
        /// Create the mock server for the current pact
        /// </summary>
        /// <param name="host">Host for the mock server</param>
        /// <param name="port">Port for the mock server, or null to allocate one automatically</param>
        /// <param name="tls">Enable TLS</param>
        /// <param name="transport">The transport to use (i.e. http, https, grpc). Must be a valid UTF-8 NULL-terminated string, or NULL or empty, in which case http will be used.</param>
        /// <returns>Mock server port</returns>
        /// <exception cref="InvalidOperationException">Failed to start mock server</exception>
        public IMockServerDriver CreateMockServer(string host, int? port, bool tls, string transport = null)
        {
            int result = NativeInterop.CreateMockServerForTransport(this.pact, host, (ushort)port.GetValueOrDefault(0), transport, null);

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
