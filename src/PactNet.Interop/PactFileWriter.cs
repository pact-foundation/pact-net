using System;

namespace PactNet.Interop
{
    /// <summary>
    /// Wrapper for interpreting results of pact file write operations
    /// </summary>
    public static class PactFileWriter
    {
        /// <summary>
        /// Write the pact file to disk
        /// </summary>
        /// <param name="pact">Pact handle for the file to be written.</param>
        /// <param name="directory">Directory of the pact file</param>
        /// <returns>Status code</returns>
        /// <exception cref="InvalidOperationException">Failed to write pact file</exception>
        public static void WritePactFile(PactHandle pact, string directory)
        {
            var result = NativeInterop.WritePactFile(pact, directory, false);
            ThrowExceptionOnFailure(result);
        }

        /// <summary>
        /// Write the pact file to disk
        /// </summary>
        /// <param name="port"></param>
        /// <param name="directory">Directory of the pact file</param>
        public static void WritePactFileForPort(int port, string directory)
        {
            var result = NativeInterop.WritePactFileForPort(port, directory, false);
            ThrowExceptionOnFailure(result);
        }

        private static void ThrowExceptionOnFailure(int result)
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
    }
}
