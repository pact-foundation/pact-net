using System.Runtime.InteropServices;

namespace PactNet.Interop;

internal static class LoggingInterop
{
    private const string DllName = "pact_ffi";

    [DllImport(DllName, EntryPoint = "pactffi_log_to_buffer")]
    public static extern int LogToBuffer(LevelFilter levelFilter);

    [DllImport(DllName, EntryPoint = "pactffi_fetch_log_buffer")]
    public static extern string FetchLogBuffer(string logId);
}
