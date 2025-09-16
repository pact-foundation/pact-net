using System;
using System.Runtime.InteropServices;

namespace PactNet.Interop;

public static class LogLevelExtensions
{
    private static readonly object LogLocker = new object();
    private static bool LogInitialised = false;

    /// <summary>
    /// Initialise logging in the native library
    /// </summary>
    /// <param name="level">Log level</param>
    /// <exception cref="ArgumentOutOfRangeException">Invalid log level</exception>
    /// <remarks>Logging can only be initialised **once**. Subsequent calls will have no effect</remarks>
    public static void InitialiseLogging(this PactLogLevel level)
    {
        lock (LogLocker)
        {
            if (LogInitialised)
            {
                return;
            }

            LoggingInterop.LogToBuffer(level switch
            {
                PactLogLevel.Trace => LevelFilter.Trace,
                PactLogLevel.Debug => LevelFilter.Debug,
                PactLogLevel.Information => LevelFilter.Info,
                PactLogLevel.Warn => LevelFilter.Warn,
                PactLogLevel.Error => LevelFilter.Error,
                PactLogLevel.None => LevelFilter.Off,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Invalid log level")
            });

            LogInitialised = true;
        }

    }

    private static class LoggingInterop
    {
        private const string DllName = "pact_ffi";

        [DllImport(DllName, EntryPoint = "pactffi_log_to_buffer")]
        public static extern int LogToBuffer(LevelFilter levelFilter);
    }
}
