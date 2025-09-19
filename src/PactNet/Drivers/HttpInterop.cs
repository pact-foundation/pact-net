using System;
using System.Runtime.InteropServices;
using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Interop definitions to the Pact FFI library
    /// </summary>
    internal static class HttpInterop
    {
        private const string DllName = "pact_ffi";

        [DllImport(DllName, EntryPoint = "pactffi_new_interaction")]
        public static extern InteractionHandle NewInteraction(PactHandle pact, string description);

        [DllImport(DllName, EntryPoint = "pactffi_with_request")]
        public static extern bool WithRequest(InteractionHandle interaction, string method, string path);

        [DllImport(DllName, EntryPoint = "pactffi_with_query_parameter_v2")]
        public static extern bool WithQueryParameter(InteractionHandle interaction, string name, UIntPtr index, string value);

        [DllImport(DllName, EntryPoint = "pactffi_with_header_v2")]
        public static extern bool WithHeader(InteractionHandle interaction, InteractionPart part, string name, UIntPtr index, string value);

        [DllImport(DllName, EntryPoint = "pactffi_response_status")]
        public static extern bool ResponseStatus(InteractionHandle interaction, ushort status);

        [DllImport(DllName, EntryPoint = "pactffi_with_body")]
        public static extern bool WithBody(InteractionHandle interaction, InteractionPart part, string contentType, string body);
    }
}
