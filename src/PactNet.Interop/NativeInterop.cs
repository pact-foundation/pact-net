using System;
using System.Runtime.InteropServices;

namespace PactNet.Interop
{
    /// <summary>
    /// Interop definitions to the Pact FFI library
    /// </summary>
    public static class NativeInterop
    {
        private const string DllName = "pact_ffi";

        #region Http Interop Support

        [DllImport(DllName, EntryPoint = "pactffi_fetch_log_buffer")]
        public static extern string FetchLogBuffer(string logId);

        [DllImport(DllName, EntryPoint = "pactffi_new_pact")]
        public static extern PactHandle NewPact(string consumerName, string providerName);

        [DllImport(DllName, EntryPoint = "pactffi_with_specification")]
        public static extern bool WithSpecification(PactHandle pact, PactSpecification version);

        [DllImport(DllName, EntryPoint = "pactffi_new_interaction")]
        public static extern InteractionHandle NewInteraction(PactHandle pact, string description);

        [DllImport(DllName, EntryPoint = "pactffi_given")]
        public static extern bool Given(InteractionHandle interaction, string description);

        [DllImport(DllName, EntryPoint = "pactffi_given_with_param")]
        public static extern bool GivenWithParam(InteractionHandle interaction, string description, string name, string value);

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

        [DllImport(DllName, EntryPoint = "pactffi_free_string")]
        public static extern void FreeString(IntPtr s);

        [DllImport(DllName, EntryPoint = "pactffi_verify")]
        public static extern int Verify(string args);

        #endregion Http Interop Support

        #region Messaging Interop Support

        [DllImport(DllName, EntryPoint = "pactffi_new_sync_message_interaction")]
        public static extern uint NewSyncMessageInteraction(PactHandle pact, string description);

        [DllImport(DllName, EntryPoint = "pactffi_with_message_pact_metadata")]
        public static extern void WithMessagePactMetadata(PactHandle pact, string @namespace, string name, string value);

        [DllImport(DllName, EntryPoint = "pactffi_new_message_interaction")]
        public static extern InteractionHandle NewMessageInteraction(PactHandle pact, string description);

        [DllImport(DllName, EntryPoint = "pactffi_message_expects_to_receive")]
        public static extern void MessageExpectsToReceive(InteractionHandle message, string description);

        [DllImport(DllName, EntryPoint = "pactffi_message_with_metadata")]
        public static extern void MessageWithMetadata(InteractionHandle message, string key, string value);

        [DllImport(DllName, EntryPoint = "pactffi_message_with_contents")]
        public static extern void MessageWithContents(InteractionHandle message, string contentType, string body, UIntPtr size);

        [DllImport(DllName, EntryPoint = "pactffi_message_reify")]
        public static extern IntPtr MessageReify(InteractionHandle message);

        #endregion Http Interop Support
    }
}
