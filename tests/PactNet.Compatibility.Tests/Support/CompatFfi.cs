using System;
using System.Runtime.InteropServices;
using PactNet.Interop;

namespace PactNet.Compatibility.Tests.Support
{
    /// <summary>
    /// FFI calls PactNet does not bind. Resolved through <see cref="NativeLibraryOverride"/> like PactNet's own.
    /// </summary>
    internal static class CompatFfi
    {
        // PactNet has no binary body API: WithBody(string, contentType) marshals a NUL-terminated C string
        [DllImport("pact_ffi", EntryPoint = "pactffi_with_binary_body")]
        public static extern bool WithBinaryBody(InteractionHandle interaction, InteractionPart part, string contentType, byte[] body, UIntPtr size);
    }
}
