using System;
using System.Runtime.InteropServices;

namespace PactNet.Interop
{
    /// <summary>
    /// Interop definitions to the Pact FFI library
    /// </summary>
    internal static
#if NET7_0_OR_GREATER
        partial
#endif
        class NativeInterop
    {
        private const string DllName = "pact_ffi";

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_log_to_buffer")]
        public static partial int LogToBuffer(LevelFilter levelFilter);
#else
        [DllImport(DllName, EntryPoint = "pactffi_log_to_buffer")]
        public static extern int LogToBuffer(LevelFilter levelFilter);
#endif

        #region Http Interop Support

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_create_mock_server_for_transport", StringMarshalling = StringMarshalling.Utf8)]
        public static partial int CreateMockServerForTransport(PactHandle pact, string addrStr, ushort port, string transport, string transportConfig);
#else
        [DllImport(DllName, EntryPoint = "pactffi_create_mock_server_for_transport")]
        public static extern int CreateMockServerForTransport(PactHandle pact, string addrStr, ushort port, string transport, string transportConfig);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_mock_server_mismatches")]
        public static partial IntPtr MockServerMismatches(int mockServerPort);
#else
        [DllImport(DllName, EntryPoint = "pactffi_mock_server_mismatches")]
        public static extern IntPtr MockServerMismatches(int mockServerPort);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_mock_server_logs")]
        public static partial IntPtr MockServerLogs(int mockServerPort);
#else
        [DllImport(DllName, EntryPoint = "pactffi_mock_server_logs")]
        public static extern IntPtr MockServerLogs(int mockServerPort);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_cleanup_mock_server")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool CleanupMockServer(int mockServerPort);
#else
        [DllImport(DllName, EntryPoint = "pactffi_cleanup_mock_server")]
        public static extern bool CleanupMockServer(int mockServerPort);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_pact_handle_write_file", StringMarshalling = StringMarshalling.Utf8)]
        public static partial int WritePactFile(PactHandle pact, string directory, [MarshalAs(UnmanagedType.I1)] bool overwrite);
#else
        [DllImport(DllName, EntryPoint = "pactffi_pact_handle_write_file")]
        public static extern int WritePactFile(PactHandle pact, string directory, bool overwrite);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_fetch_log_buffer", StringMarshalling = StringMarshalling.Utf8)]
        public static partial string FetchLogBuffer(string logId);
#else
        [DllImport(DllName, EntryPoint = "pactffi_fetch_log_buffer")]
        public static extern string FetchLogBuffer(string logId);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_new_pact", StringMarshalling = StringMarshalling.Utf8)]
        public static partial PactHandle NewPact(string consumerName, string providerName);
#else
        [DllImport(DllName, EntryPoint = "pactffi_new_pact")]
        public static extern PactHandle NewPact(string consumerName, string providerName);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_with_specification")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool WithSpecification(PactHandle pact, PactSpecification version);
#else
        [DllImport(DllName, EntryPoint = "pactffi_with_specification")]
        public static extern bool WithSpecification(PactHandle pact, PactSpecification version);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_new_interaction", StringMarshalling = StringMarshalling.Utf8)]
        public static partial InteractionHandle NewInteraction(PactHandle pact, string description);
#else
        [DllImport(DllName, EntryPoint = "pactffi_new_interaction")]
        public static extern InteractionHandle NewInteraction(PactHandle pact, string description);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_given", StringMarshalling = StringMarshalling.Utf8)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool Given(InteractionHandle interaction, string description);
#else
        [DllImport(DllName, EntryPoint = "pactffi_given")]
        public static extern bool Given(InteractionHandle interaction, string description);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_given_with_param", StringMarshalling = StringMarshalling.Utf8)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool GivenWithParam(InteractionHandle interaction, string description, string name, string value);
#else
        [DllImport(DllName, EntryPoint = "pactffi_given_with_param")]
        public static extern bool GivenWithParam(InteractionHandle interaction, string description, string name, string value);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_with_request", StringMarshalling = StringMarshalling.Utf8)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool WithRequest(InteractionHandle interaction, string method, string path);
#else
        [DllImport(DllName, EntryPoint = "pactffi_with_request")]
        public static extern bool WithRequest(InteractionHandle interaction, string method, string path);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_with_query_parameter_v2", StringMarshalling = StringMarshalling.Utf8)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool WithQueryParameter(InteractionHandle interaction, string name, UIntPtr index, string value);
#else
        [DllImport(DllName, EntryPoint = "pactffi_with_query_parameter_v2")]
        public static extern bool WithQueryParameter(InteractionHandle interaction, string name, UIntPtr index, string value);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_with_header_v2", StringMarshalling = StringMarshalling.Utf8)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool WithHeader(InteractionHandle interaction, InteractionPart part, string name, UIntPtr index, string value);
#else
        [DllImport(DllName, EntryPoint = "pactffi_with_header_v2")]
        public static extern bool WithHeader(InteractionHandle interaction, InteractionPart part, string name, UIntPtr index, string value);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_response_status")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool ResponseStatus(InteractionHandle interaction, ushort status);
#else
        [DllImport(DllName, EntryPoint = "pactffi_response_status")]
        public static extern bool ResponseStatus(InteractionHandle interaction, ushort status);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_with_body", StringMarshalling = StringMarshalling.Utf8)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static partial bool WithBody(InteractionHandle interaction, InteractionPart part, string contentType, string body);
#else
        [DllImport(DllName, EntryPoint = "pactffi_with_body")]
        public static extern bool WithBody(InteractionHandle interaction, InteractionPart part, string contentType, string body);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_free_string")]
        public static partial void FreeString(IntPtr s);
#else
        [DllImport(DllName, EntryPoint = "pactffi_free_string")]
        public static extern void FreeString(IntPtr s);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verify", StringMarshalling = StringMarshalling.Utf8)]
        public static partial int Verify(string args);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verify")]
        public static extern int Verify(string args);
#endif

        #endregion Http Interop Support

        #region Messaging Interop Support

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_with_message_pact_metadata", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void WithMessagePactMetadata(PactHandle pact, string @namespace, string name, string value);
#else
        [DllImport(DllName, EntryPoint = "pactffi_with_message_pact_metadata")]
        public static extern void WithMessagePactMetadata(PactHandle pact, string @namespace, string name, string value);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_new_message_interaction", StringMarshalling = StringMarshalling.Utf8)]
        public static partial InteractionHandle NewMessageInteraction(PactHandle pact, string description);
#else
        [DllImport(DllName, EntryPoint = "pactffi_new_message_interaction")]
        public static extern InteractionHandle NewMessageInteraction(PactHandle pact, string description);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_message_expects_to_receive", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void MessageExpectsToReceive(InteractionHandle message, string description);
#else
        [DllImport(DllName, EntryPoint = "pactffi_message_expects_to_receive")]
        public static extern void MessageExpectsToReceive(InteractionHandle message, string description);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_message_with_metadata", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void MessageWithMetadata(InteractionHandle message, string key, string value);
#else
        [DllImport(DllName, EntryPoint = "pactffi_message_with_metadata")]
        public static extern void MessageWithMetadata(InteractionHandle message, string key, string value);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_message_with_contents", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void MessageWithContents(InteractionHandle message, string contentType, string body, UIntPtr size);
#else
        [DllImport(DllName, EntryPoint = "pactffi_message_with_contents")]
        public static extern void MessageWithContents(InteractionHandle message, string contentType, string body, UIntPtr size);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_message_reify")]
        public static partial IntPtr MessageReify(InteractionHandle message);
#else
        [DllImport(DllName, EntryPoint = "pactffi_message_reify")]
        public static extern IntPtr MessageReify(InteractionHandle message);
#endif

        #endregion Http Interop Support

        #region Verifier Support

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_new_for_application", StringMarshalling = StringMarshalling.Utf8)]
        public static partial IntPtr VerifierNewForApplication(string name, string version);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_new_for_application")]
        public static extern IntPtr VerifierNewForApplication(string name, string version);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_shutdown")]
        public static partial IntPtr VerifierShutdown(IntPtr handle);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_shutdown")]
        public static extern IntPtr VerifierShutdown(IntPtr handle);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_set_provider_info", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void VerifierSetProviderInfo(IntPtr handle, string name, string scheme, string host, ushort port, string path);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_set_provider_info")]
        public static extern void VerifierSetProviderInfo(IntPtr handle, string name, string scheme, string host, ushort port, string path);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_add_provider_transport", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void AddProviderTransport(IntPtr handle, string protocol, ushort port, string path, string scheme);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_add_provider_transport")]
        public static extern void AddProviderTransport(IntPtr handle, string protocol, ushort port, string path, string scheme);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_set_filter_info", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void VerifierSetFilterInfo(IntPtr handle, string description, string state, byte noState);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_set_filter_info")]
        public static extern void VerifierSetFilterInfo(IntPtr handle, string description, string state, byte noState);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_set_provider_state", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void VerifierSetProviderState(IntPtr handle, string url, byte teardown, byte body);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_set_provider_state")]
        public static extern void VerifierSetProviderState(IntPtr handle, string url, byte teardown, byte body);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_set_verification_options")]
        public static partial void VerifierSetVerificationOptions(IntPtr handle,
                                                                  byte disableSslVerification,
                                                                  uint requestTimeout);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_set_verification_options")]
        public static extern void VerifierSetVerificationOptions(IntPtr handle,
                                                                 byte disableSslVerification,
                                                                 uint requestTimeout);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_set_publish_options", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void VerifierSetPublishOptions(IntPtr handle,
                                                             string providerVersion,
                                                             string buildUrl,
                                                             string[] providerTags,
                                                             ushort providerTagsLength,
                                                             string providerBranch);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_set_publish_options")]
        public static extern void VerifierSetPublishOptions(IntPtr handle,
                                                            string providerVersion,
                                                            string buildUrl,
                                                            string[] providerTags,
                                                            ushort providerTagsLength,
                                                            string providerBranch);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_set_consumer_filters", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void VerifierSetConsumerFilters(IntPtr handle, string[] consumerFilters, ushort consumerFiltersLength);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_set_consumer_filters")]
        public static extern void VerifierSetConsumerFilters(IntPtr handle, string[] consumerFilters, ushort consumerFiltersLength);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_add_custom_header", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void AddCustomHeader(IntPtr handle, string name, string value);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_add_custom_header")]
        public static extern void AddCustomHeader(IntPtr handle, string name, string value);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_add_file_source", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void VerifierAddFileSource(IntPtr handle, string file);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_add_file_source")]
        public static extern void VerifierAddFileSource(IntPtr handle, string file);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_add_directory_source", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void VerifierAddDirectorySource(IntPtr handle, string directory);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_add_directory_source")]
        public static extern void VerifierAddDirectorySource(IntPtr handle, string directory);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_url_source", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void VerifierUrlSource(IntPtr handle, string url, string username, string password, string token);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_url_source")]
        public static extern void VerifierUrlSource(IntPtr handle, string url, string username, string password, string token);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_broker_source_with_selectors", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void VerifierBrokerSourceWithSelectors(IntPtr handle,
                                                                     string url,
                                                                     string username,
                                                                     string password,
                                                                     string token,
                                                                     byte enablePending,
                                                                     string includeWipPactsSince,
                                                                     string[] providerTags,
                                                                     ushort providerTagsLength,
                                                                     string providerBranch,
                                                                     string[] consumerVersionSelectors,
                                                                     ushort consumerVersionSelectorsLength,
                                                                     string[] consumerVersionTags,
                                                                     ushort consumerVersionTagsLength);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_broker_source_with_selectors")]
        public static extern void VerifierBrokerSourceWithSelectors(IntPtr handle,
                                                                    string url,
                                                                    string username,
                                                                    string password,
                                                                    string token,
                                                                    byte enablePending,
                                                                    string includeWipPactsSince,
                                                                    string[] providerTags,
                                                                    ushort providerTagsLength,
                                                                    string providerBranch,
                                                                    string[] consumerVersionSelectors,
                                                                    ushort consumerVersionSelectorsLength,
                                                                    string[] consumerVersionTags,
                                                                    ushort consumerVersionTagsLength);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_execute")]
        public static partial int VerifierExecute(IntPtr handle);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_execute")]
        public static extern int VerifierExecute(IntPtr handle);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_logs")]
        public static partial IntPtr VerifierLogs(IntPtr handle);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_logs")]
        public static extern IntPtr VerifierLogs(IntPtr handle);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(DllName, EntryPoint = "pactffi_verifier_output")]
        public static partial IntPtr VerifierOutput(IntPtr handle, byte stripAnsi);
#else
        [DllImport(DllName, EntryPoint = "pactffi_verifier_output")]
        public static extern IntPtr VerifierOutput(IntPtr handle, byte stripAnsi);
#endif

        #endregion
    }
}
