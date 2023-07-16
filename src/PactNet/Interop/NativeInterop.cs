using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PactNet.Interop
{
    /// <summary>
    /// Interop definitions to the Pact FFI library
    /// </summary>
    internal static class NativeInterop
    {
        private const string DllName = "pact_ffi";

        [DllImport(DllName, EntryPoint = "pactffi_log_to_buffer")]
        public static extern int LogToBuffer(LevelFilter levelFilter);

        #region Http Interop Support

        [DllImport(DllName, EntryPoint = "pactffi_create_mock_server_for_pact")]
        public static extern int CreateMockServerForPact(PactHandle pact, string addrStr, bool tls);

        [DllImport(DllName, EntryPoint = "pactffi_mock_server_mismatches")]
        public static extern IntPtr MockServerMismatches(int mockServerPort);

        [DllImport(DllName, EntryPoint = "pactffi_mock_server_logs")]
        public static extern IntPtr MockServerLogs(int mockServerPort);

        [DllImport(DllName, EntryPoint = "pactffi_cleanup_mock_server")]
        public static extern bool CleanupMockServer(int mockServerPort);

        [DllImport(DllName, EntryPoint = "pactffi_pact_handle_write_file")]
        public static extern int WritePactFile(PactHandle pact, byte[] directory, bool overwrite);

        [DllImport(DllName, EntryPoint = "pactffi_fetch_log_buffer")]
        public static extern string FetchLogBuffer(byte[] logId);

        [DllImport(DllName, EntryPoint = "pactffi_new_pact")]
        public static extern PactHandle NewPact(byte[] consumerName, byte[] providerName);

        [DllImport(DllName, EntryPoint = "pactffi_with_specification")]
        public static extern bool WithSpecification(PactHandle pact, PactSpecification version);

        [DllImport(DllName, EntryPoint = "pactffi_new_interaction")]
        public static extern InteractionHandle NewInteraction(PactHandle pact, byte[] description);

        [DllImport(DllName, EntryPoint = "pactffi_given")]
        public static extern bool Given(InteractionHandle interaction, byte[] description);

        [DllImport(DllName, EntryPoint = "pactffi_given_with_param")]
        public static extern bool GivenWithParam(InteractionHandle interaction, byte[] description, byte[] name, byte[] value);

        [DllImport(DllName, EntryPoint = "pactffi_with_request")]
        public static extern bool WithRequest(InteractionHandle interaction, string method, string path);

        [DllImport(DllName, EntryPoint = "pactffi_with_query_parameter_v2")]
        public static extern bool WithQueryParameter(InteractionHandle interaction, byte[] name, UIntPtr index, byte[] value);

        [DllImport(DllName, EntryPoint = "pactffi_with_header_v2")]
        public static extern bool WithHeader(InteractionHandle interaction, InteractionPart part, byte[] name, UIntPtr index, byte[] value);

        [DllImport(DllName, EntryPoint = "pactffi_response_status")]
        public static extern bool ResponseStatus(InteractionHandle interaction, ushort status);

        [DllImport(DllName, EntryPoint = "pactffi_with_body")]
        public static extern bool WithBody(InteractionHandle interaction, InteractionPart part, byte[] contentType, byte[] body);

        #endregion Http Interop Support

        #region Messaging Interop Support

        [DllImport(DllName, EntryPoint = "pactffi_with_message_pact_metadata")]
        public static extern void WithMessagePactMetadata(PactHandle pact, byte[] @namespace, byte[] name, byte[] value);

        [DllImport(DllName, EntryPoint = "pactffi_new_message_interaction")]
        public static extern InteractionHandle NewMessageInteraction(PactHandle pact, byte[] description);

        [DllImport(DllName, EntryPoint = "pactffi_message_expects_to_receive")]
        public static extern void MessageExpectsToReceive(InteractionHandle message, byte[] description);

        [DllImport(DllName, EntryPoint = "pactffi_message_with_metadata")]
        public static extern void MessageWithMetadata(InteractionHandle message, byte[] key, byte[] value);

        [DllImport(DllName, EntryPoint = "pactffi_message_with_contents")]
        public static extern void MessageWithContents(InteractionHandle message, byte[] contentType, byte[] body, UIntPtr size);

        [DllImport(DllName, EntryPoint = "pactffi_message_reify")]
        public static extern IntPtr MessageReify(InteractionHandle message);

        #endregion Http Interop Support

        #region Verifier Support

        [DllImport(DllName, EntryPoint = "pactffi_verifier_new_for_application")]
        public static extern IntPtr VerifierNewForApplication(byte[] name, byte[] version);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_shutdown")]
        public static extern IntPtr VerifierShutdown(IntPtr handle);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_set_provider_info")]
        public static extern void VerifierSetProviderInfo(IntPtr handle, byte[] name, string scheme, string host, ushort port, string path);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_add_provider_transport")]
        public static extern void AddProviderTransport(IntPtr handle, string protocol, ushort port, string path, string scheme);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_set_filter_info")]
        public static extern void VerifierSetFilterInfo(IntPtr handle, byte[] description, byte[] state, byte noState);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_set_provider_state")]
        public static extern void VerifierSetProviderState(IntPtr handle, string url, byte teardown, byte body);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_set_verification_options")]
        public static extern void VerifierSetVerificationOptions(IntPtr handle,
                                                                 byte disableSslVerification,
                                                                 uint requestTimeout);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_set_publish_options")]
        public static extern void VerifierSetPublishOptions(IntPtr handle,
                                                            byte[] providerVersion,
                                                            byte[] buildUrl,
                                                            string[] providerTags,
                                                            ushort providerTagsLength,
                                                            byte[] providerBranch);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_set_consumer_filters")]
        public static extern void VerifierSetConsumerFilters(IntPtr handle, string[] consumerFilters, ushort consumerFiltersLength);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_add_custom_header")]
        public static extern void AddCustomHeader(IntPtr handle, byte[] name, byte[] value);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_add_file_source")]
        public static extern void VerifierAddFileSource(IntPtr handle, byte[] file);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_add_directory_source")]
        public static extern void VerifierAddDirectorySource(IntPtr handle, byte[] directory);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_url_source")]
        public static extern void VerifierUrlSource(IntPtr handle, byte[] url, byte[] username, byte[] password, byte[] token);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_broker_source_with_selectors")]
        public static extern void VerifierBrokerSourceWithSelectors(IntPtr handle,
                                                                    byte[] url,
                                                                    byte[] username,
                                                                    byte[] password,
                                                                    byte[] token,
                                                                    byte enablePending,
                                                                    string includeWipPactsSince,
                                                                    string[] providerTags,
                                                                    ushort providerTagsLength,
                                                                    byte[] providerBranch,
                                                                    string[] consumerVersionSelectors,
                                                                    ushort consumerVersionSelectorsLength,
                                                                    string[] consumerVersionTags,
                                                                    ushort consumerVersionTagsLength);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_execute")]
        public static extern int VerifierExecute(IntPtr handle);

        [DllImport(DllName, EntryPoint = "pactffi_verifier_logs")]
        public static extern IntPtr VerifierLogs(IntPtr handle);


        [DllImport(DllName, EntryPoint = "pactffi_verifier_output")]
        public static extern IntPtr VerifierOutput(IntPtr handle, byte stripAnsi);

        #endregion

        public static byte[] StringToUtf8(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return Array.Empty<byte>();
            }

            var bytes = Encoding.UTF8.GetBytes(s);
            return bytes;
        }
    }
}
