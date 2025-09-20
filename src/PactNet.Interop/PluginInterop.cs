using System.Runtime.InteropServices;

namespace PactNet.Interop;

internal class PluginInterop
{
    private const string DllName = "pact_ffi";

    [DllImport(DllName, EntryPoint = "pactffi_new_sync_message_interaction")]
    public static extern InteractionHandle NewSyncMessageInteraction(PactHandle pact, string description);

    [DllImport(DllName, EntryPoint = "pactffi_using_plugin")]
    public static extern uint PluginAdd(PactHandle pact, string name, string version);

    [DllImport(DllName, EntryPoint = "pactffi_interaction_contents")]
    public static extern uint PluginInteractionContents(InteractionHandle interaction, InteractionPart part, string contentType, string body);

    [DllImport(DllName, EntryPoint = "pactffi_cleanup_plugins")]
    public static extern void PluginCleanup(PactHandle pact);
}
