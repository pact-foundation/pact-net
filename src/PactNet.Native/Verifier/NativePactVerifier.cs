using System;
using System.Runtime.InteropServices;
using PactNet.Native.Interop;

namespace PactNet.Native.Verifier
{
    /// <summary>
    /// Native pact verifier
    /// </summary>
    internal class NativePactVerifier : IVerifierProvider
    {
        /// <summary>
        /// Verify the pact from the given args
        /// </summary>
        /// <param name="args">Verifier args</param>
        /// <returns>Verifier result</returns>
        public PactVerifierResult Verify(string args)
        {
            int result = NativeInterop.Verify(args);

            return result switch
            {
                0 => PactVerifierResult.Success,
                1 => PactVerifierResult.Failure,
                2 => PactVerifierResult.NullPointer,
                3 => PactVerifierResult.Panic,
                4 => PactVerifierResult.InvalidArguments,
                _ => PactVerifierResult.UnknownError
            };
        }

        /// <summary>
        /// Get the logs for the given provider
        /// </summary>
        /// <param name="provider">Name of the provider</param>
        /// <returns>Verifier logs</returns>
        public string VerifierLogs(string provider)
        {
            IntPtr logsPtr = NativeInterop.VerifierLogsForProvider(provider);

            return logsPtr == IntPtr.Zero
                       ? "ERROR: Unable to retrieve verifier logs"
                       : Marshal.PtrToStringAnsi(logsPtr);
        }
    }
}
