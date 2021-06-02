using System;
using System.Runtime.InteropServices;

namespace PactNet.Native
{
    /// <summary>
    /// Native pact verifier
    /// </summary>
    internal class NativePactVerifier : IVerifierProvider
    {
        /// <summary>
        /// Static constructor for <see cref="NativePactVerifier"/>
        /// </summary>
        static NativePactVerifier()
        {
            // TODO: make this configurable somehow, except it applies once for the entire native lifetime, so dunno
            Environment.SetEnvironmentVariable("LOG_LEVEL", "DEBUG");
            Interop.Init("LOG_LEVEL");
        }

        /// <summary>
        /// Verify the pact from the given args
        /// </summary>
        /// <param name="args">Verifier args</param>
        public void Verify(string args)
        {
            int result = Interop.Verify(args);

            if (result == 0)
            {
                return;
            }

            throw result switch
            {
                1 => new PactFailureException("The verification process failed, see output for errors"),
                2 => new PactFailureException("A null pointer was received"),
                3 => new PactFailureException("The method panicked"),
                4 => new PactFailureException("Invalid arguments were provided to the verification process"),
                _ => new PactFailureException($"An unknown error occurred with error code {result}")
            };
        }

        /// <summary>
        /// P/Invoke bondings to the pact verifier FFI library
        /// </summary>
        private static class Interop
        {
            private const string dllName = "pact_verifier_ffi";

            [DllImport(dllName, EntryPoint = "init")]
            public static extern void Init(string logEnvVar);

            [DllImport(dllName, EntryPoint = "free_string")]
            public static extern void FreeString(string s);

            [DllImport(dllName, EntryPoint = "verify")]
            public static extern int Verify(string args);
        }
    }
}
