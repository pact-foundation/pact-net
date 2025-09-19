using System;
using System.Runtime.InteropServices;
using PactNet.Interop;

namespace PactNet.Drivers;

internal static class MessagingInterop
{
    private const string DllName = "pact_ffi";

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
}
