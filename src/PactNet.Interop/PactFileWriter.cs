using System;
using System.Runtime.InteropServices;

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
            var result = PactFileInterop.WritePactFile(pact, directory, false);
            ThrowExceptionOnFailure(result);
        }

        /// <summary>
        /// Write the pact file to disk
        /// </summary>
        /// <param name="port"></param>
        /// <param name="directory">Directory of the pact file</param>
        public static void WritePactFileForPort(int port, string directory)
        {
            var result = PactFileInterop.WritePactFileForPort(port, directory, false);
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

        private static class PactFileInterop
        {
            private const string DllName = "pact_ffi";

            [DllImport(DllName, EntryPoint = "pactffi_pact_handle_write_file")]
            internal static extern int WritePactFile(PactHandle pact, string directory, bool overwrite);

            [DllImport(DllName, EntryPoint = "pactffi_write_pact_file")]
            internal static extern int WritePactFileForPort(int port, string directory, bool overwrite);
        }
    }
}
