using System;
using System.Runtime.InteropServices;
using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for managing a HTTP mock server
    /// </summary>
    internal class MockServerDriver : IMockServerDriver
    {
        private readonly int port;

        /// <summary>
        /// Mock server URI
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
            this.port = port;
        }

        /// <summary>
        /// Get a string representing the mismatches following interaction testing
        /// </summary>
        /// <returns>Mismatch string</returns>
        public string MockServerMismatches()
        {
            IntPtr matchesPtr = NativeInterop.MockServerMismatches(this.port);

            return matchesPtr == IntPtr.Zero
                       ? string.Empty
                       : Marshal.PtrToStringAnsi(matchesPtr);
        }

        /// <summary>
        /// Get a string representing the mock server logs following interaction testing
        /// </summary>
        /// <returns>Log string</returns>
        public string MockServerLogs()
        {
            IntPtr logsPtr = NativeInterop.MockServerLogs(this.port);

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
            NativeInterop.CleanupMockServer(this.port);
        }
    }
}
