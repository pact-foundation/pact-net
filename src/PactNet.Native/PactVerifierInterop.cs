using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PactNet.Native
{
    /// <summary>
    /// Pact verifier interop
    /// </summary>
    internal static class PactVerifierInterop
    {
        private const string dllName = "pact_verifier_ffi";

        /// <summary>
        /// Static constructor for <see cref="MockServerInterop"/>
        /// </summary>
        static PactVerifierInterop()
        {
            // TODO: make this configurable somehow, except it applies once for the entire native lifetime, so dunno
            Environment.SetEnvironmentVariable("LOG_LEVEL", "DEBUG");
            Init("LOG_LEVEL");
        }

        [DllImport(dllName, EntryPoint = "init")]
        public static extern void Init(string logEnvVar);

        [DllImport(dllName, EntryPoint = "free_string")]
        public static extern void FreeString(string s);

        [DllImport(dllName, EntryPoint = "verify")]
        public static extern int Verify(string args);
    }
}