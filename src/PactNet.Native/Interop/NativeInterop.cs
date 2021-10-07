using System;
using System.Runtime.InteropServices;

namespace PactNet.Native.Interop
{
    /// <summary>
    /// Interop definitions to the Pact FFI library
    /// </summary>
    internal static class NativeInterop
    {
        private const string dllName = "pact_ffi";

        /// <summary>
        /// Static initialiser for the Pact FFI library
        /// </summary>
        static NativeInterop()
        {
            // TODO: Make this configurable and specified by the user
            LogToBuffer(LevelFilter.Debug);
        }

        #region Http Interop Support

        [DllImport(dllName, EntryPoint = "pactffi_log_to_buffer")]
        public static extern int LogToBuffer(LevelFilter levelFilter);

        [DllImport(dllName, EntryPoint = "pactffi_create_mock_server_for_pact")]
        public static extern int CreateMockServerForPact(PactHandle pact, string addrStr, bool tls);

        [DllImport(dllName, EntryPoint = "pactffi_mock_server_mismatches")]
        public static extern IntPtr MockServerMismatches(int mockServerPort);

        [DllImport(dllName, EntryPoint = "pactffi_mock_server_logs")]
        public static extern IntPtr MockServerLogs(int mockServerPort);

        [DllImport(dllName, EntryPoint = "pactffi_cleanup_mock_server")]
        public static extern bool CleanupMockServer(int mockServerPort);

        [DllImport(dllName, EntryPoint = "pactffi_write_pact_file")]
        public static extern int WritePactFile(int mockServerPort, string directory, bool overwrite);

        [DllImport(dllName, EntryPoint = "pactffi_new_pact")]
        public static extern PactHandle NewPact(string consumerName, string providerName);

        [DllImport(dllName, EntryPoint = "pactffi_with_specification")]
        public static extern bool WithSpecification(PactHandle pact, PactSpecification version);

        [DllImport(dllName, EntryPoint = "pactffi_new_interaction")]
        public static extern InteractionHandle NewInteraction(PactHandle pact, string description);

        [DllImport(dllName, EntryPoint = "pactffi_given")]
        public static extern bool Given(InteractionHandle interaction, string description);

        [DllImport(dllName, EntryPoint = "pactffi_given_with_param")]
        public static extern bool GivenWithParam(InteractionHandle interaction, string description, string name, string value);

        [DllImport(dllName, EntryPoint = "pactffi_with_request")]
        public static extern bool WithRequest(InteractionHandle interaction, string method, string path);

        [DllImport(dllName, EntryPoint = "pactffi_with_query_parameter")]
        public static extern bool WithQueryParameter(InteractionHandle interaction, string name, UIntPtr index, string value);

        [DllImport(dllName, EntryPoint = "pactffi_with_header")]
        public static extern bool WithHeader(InteractionHandle interaction, InteractionPart part, string name, UIntPtr index, string value);

        [DllImport(dllName, EntryPoint = "pactffi_response_status")]
        public static extern bool ResponseStatus(InteractionHandle interaction, ushort status);

        [DllImport(dllName, EntryPoint = "pactffi_with_body")]
        public static extern bool WithBody(InteractionHandle interaction, InteractionPart part, string contentType, string body);

        [DllImport(dllName, EntryPoint = "pactffi_free_string")]
        public static extern void FreeString(IntPtr s);

        [DllImport(dllName, EntryPoint = "pactffi_verify")]
        public static extern int Verify(string args);

        [DllImport(dllName, EntryPoint = "pactffi_verifier_logs_for_provider")]
        public static extern IntPtr VerifierLogsForProvider(string provider);

        #endregion Http Interop Support

        #region Messaging Interop Support

        [DllImport(dllName, EntryPoint = "pactffi_write_message_pact_file")]
        public static extern int WriteMessagePactFile(MessagePactHandle pact, string directory, bool overwrite);

        [DllImport(dllName, EntryPoint = "pactffi_with_message_pact_metadata")]
        public static extern bool WithMessagePactMetadata(MessagePactHandle pact, string @namespace, string name, string value);

        [DllImport(dllName, EntryPoint = "pactffi_new_message_pact")]
        public static extern MessagePactHandle NewMessagePact(string consumerName, string providerName);

        [DllImport(dllName, EntryPoint = "pactffi_new_message")]
        public static extern MessageHandle NewMessage(MessagePactHandle pact, string description);

        [DllImport(dllName, EntryPoint = "pactffi_message_expects_to_receive")]
        public static extern bool MessageExpectsToReceive(MessageHandle message, string description);

        [DllImport(dllName, EntryPoint = "pactffi_message_given")]
        public static extern bool MessageGiven(MessageHandle message, string description);

        [DllImport(dllName, EntryPoint = "pactffi_message_given_with_param")]
        public static extern bool MessageGivenWithParam(MessageHandle message, string description, string name, string value);

        [DllImport(dllName, EntryPoint = "pactffi_message_with_metadata")]
        public static extern bool MessageWithMetadata(MessageHandle message, string key, string value);

        [DllImport(dllName, EntryPoint = "pactffi_message_with_contents")]
        public static extern bool MessageWithContents(MessageHandle message, string contentType, string body, UIntPtr size);

        [DllImport(dllName, EntryPoint = "pactffi_message_reify")]
        public static extern IntPtr MessageReify(MessageHandle message);

        #endregion Http Interop Support
    }
}
