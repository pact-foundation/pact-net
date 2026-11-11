using System;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for writing completed pact files containing interactions
    /// </summary>
    internal interface ICompletedPactDriver
    {
        /// <summary>
        /// Write the pact file to disk
        /// </summary>
        /// <param name="directory">Directory of the pact file</param>
        /// <returns>Status code</returns>
        /// <exception cref="InvalidOperationException">Failed to write pact file</exception>
        void WritePactFile(string directory);

        /// <summary>
        /// Write the pact file to disk
        /// </summary>
        /// <param name="port">Port of the mock server</param>
        /// <param name="directory">Directory of the pact file</param>
        void WritePactFile(int port, string directory);

        /// <summary>
        /// Create the mock server for the current pact
        /// </summary>
        /// <param name="host">Host for the mock server</param>
        /// <param name="port">Port for the mock server, or null to allocate one automatically</param>
        /// <param name="tls">Enable TLS</param>
        /// <param name="transport">The transport to use (i.e. http, https, grpc). Must be a valid UTF-8 NULL-terminated string, or NULL or empty, in which case http will be used.</param>
        /// <returns>Mock server port</returns>
        /// <exception cref="InvalidOperationException">Failed to start mock server</exception>
        IMockServerDriver CreateMockServer(string host, int? port, bool tls, string transport = null);
    }
}
