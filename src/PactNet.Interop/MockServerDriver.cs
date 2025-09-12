using System;
using System.Runtime.InteropServices;
using PactNet.Drivers;

namespace PactNet.Interop
{
    /// <summary>
    /// Driver for managing a HTTP mock server
    /// </summary>
    internal class MockServerDriver : IMockServerDriver
    {
        /// <summary>
        /// <inheritdoc cref="Port"/>
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// <inheritdoc cref="Uri"/>
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="MockServerDriver"/> class.
        /// </summary>
        /// <param name="host">Mock server host</param>
        /// <param name="port">Mock server port</param>
        /// <param name="tls">Is the mock server hosted via TLS?</param>
        internal MockServerDriver(string host, int port, bool tls)
        {
            string scheme = tls ? "https" : "http";
            this.Uri = new Uri($"{scheme}://{host}:{port}");
            this.Port = port;
        }

        /// <summary>
        /// <inheritdoc cref="MockServerMatched"/>
        /// </summary>
        public bool MockServerMatched()
        {
            return MockServerInterop.MockServerMatched(Port);
        }

        /// <summary>
        /// <inheritdoc cref="MockServerMismatches"/>
        /// </summary>
        public string MockServerMismatches()
        {
            IntPtr matchesPtr = MockServerInterop.MockServerMismatches(this.Port);

            return matchesPtr == IntPtr.Zero
                       ? string.Empty
                       : Marshal.PtrToStringAnsi(matchesPtr);
        }

        /// <summary>
        /// <inheritdoc cref="MockServerLogs"/>
        /// </summary>
        public string MockServerLogs()
        {
            IntPtr logsPtr = MockServerInterop.MockServerLogs(this.Port);

            return logsPtr == IntPtr.Zero
                       ? "ERROR: Unable to retrieve mock server logs"
                       : Marshal.PtrToStringAnsi(logsPtr);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~MockServerDriver()
        {
            this.ReleaseUnmanagedResources();
        }

        /// <summary>
        /// Release unmanaged resources
        /// </summary>
        private void ReleaseUnmanagedResources()
        {
            MockServerInterop.CleanupMockServer(this.Port);
        }
    }
}
