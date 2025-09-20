using System.Runtime.InteropServices;

namespace PactNet.Interop;

internal static class PactInterop
{
    private const string DllName = "pact_ffi";

    [DllImport(DllName, EntryPoint = "pactffi_new_pact")]
    public static extern PactHandle NewPact(string consumerName, string providerName);

    [DllImport(DllName, EntryPoint = "pactffi_with_specification")]
    public static extern bool WithSpecification(PactHandle pact, PactSpecification version);

    [DllImport(DllName, EntryPoint = "pactffi_given")]
    public static extern bool Given(InteractionHandle interaction, string description);

    [DllImport(DllName, EntryPoint = "pactffi_given_with_param")]
    public static extern bool GivenWithParam(InteractionHandle interaction, string description, string name, string value);
}
