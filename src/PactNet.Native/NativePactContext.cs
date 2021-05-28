using System;
using System.Runtime.InteropServices;

namespace PactNet.Native
{
    /// <summary>
    /// Native mock server context
    /// </summary>
    public class NativePactContext : IPactContext
    {
        private readonly PactConfig config;

        /// <summary>
        /// URI for the mock server
        /// </summary>
        public Uri MockServerUri { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="NativePactContext"/> class.
        /// </summary>
        /// <param name="mockServerUri">Mock server URI</param>
        /// <param name="config">Pact config</param>
        internal NativePactContext(Uri mockServerUri, PactConfig config)
        {
            this.config = config;
            this.MockServerUri = mockServerUri;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            IntPtr errorPtr = MockServerInterop.MockServerMismatches(this.MockServerUri.Port);
            string errors = string.Empty;

            if (errorPtr != IntPtr.Zero)
            {
                errors = Marshal.PtrToStringAnsi(errorPtr);
            }

            if (string.IsNullOrWhiteSpace(errors) || errors == "[]" )
            {
                int result = MockServerInterop.WritePactFile(this.MockServerUri.Port, this.config.PactDir, this.config.Overwrite);

                switch (result)
                {
                    case 0: return;
                    case 1: throw new InvalidOperationException("The pact reference library panicked");
                    case 2: throw new InvalidOperationException("The pact file was not able to be written");
                    case 3: throw new InvalidOperationException("A mock server with the provided port was not found");
                    default: throw new InvalidOperationException($"Unknown error from backend: {result}");
                }
            }

            this.config.WriteLine("Verification mismatches:");
            this.config.WriteLine(string.Empty);
            this.config.WriteLine(errors);

            throw new PactFailureException("Pact verification failed. See output for details");
        }
    }
}