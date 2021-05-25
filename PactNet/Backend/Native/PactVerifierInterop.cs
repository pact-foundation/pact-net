using System.Runtime.InteropServices;

namespace PactNet.Backend.Native
{
    /// <summary>
    /// Pact verifier interop
    /// </summary>
    internal static class PactVerifierInterop
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