using System;
using System.Runtime.InteropServices;

namespace PactNet.Interop;

internal class MockServerInterop
{
    private const string DllName = "pact_ffi";

    [DllImport(DllName, EntryPoint = "pactffi_create_mock_server_for_transport")]
    internal static extern int CreateMockServerForTransport(PactHandle pact, string addrStr, ushort port, string transport, string transportConfig);

    [DllImport(DllName, EntryPoint = "pactffi_mock_server_matched")]
    internal static extern bool MockServerMatched(int mockServerPort);

    [DllImport(DllName, EntryPoint = "pactffi_mock_server_mismatches")]
    internal static extern IntPtr MockServerMismatches(int mockServerPort);

    [DllImport(DllName, EntryPoint = "pactffi_mock_server_logs")]
    internal static extern IntPtr MockServerLogs(int mockServerPort);

    [DllImport(DllName, EntryPoint = "pactffi_cleanup_mock_server")]
    internal static extern bool CleanupMockServer(int mockServerPort);
}
