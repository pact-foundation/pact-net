using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using PactNet.Interop;

namespace PactNet.Compatibility.Tests.Support
{
    /// <summary>
    /// Lets <c>PACT_FFI_LIB</c> point at a specific pact_ffi build (a library file, or a directory containing one)
    /// instead of the copy PactNet would normally load from <c>build/&lt;os&gt;/&lt;arch&gt;/</c>
    /// </summary>
    internal static class NativeLibraryOverride
    {
        public const string EnvironmentVariable = "PACT_FFI_LIB";
        private const string LibraryName = "pact_ffi";

        private static readonly object Sync = new();
        private static bool installed;

        /// <summary>
        /// Resolved library path, or null when the default resolution is being used
        /// </summary>
        public static string LibraryPath { get; private set; }

        public static void Install()
        {
            lock (Sync)
            {
                if (installed)
                {
                    return;
                }

                installed = true;

                string configured = Environment.GetEnvironmentVariable(EnvironmentVariable);
                if (string.IsNullOrWhiteSpace(configured))
                {
                    return;
                }

                LibraryPath = ResolvePath(configured);

                // PactNet's own P/Invokes, plus the few extra ones this test assembly declares
                SetResolver(typeof(NativeInterop).Assembly);
                SetResolver(typeof(NativeLibraryOverride).Assembly);
            }
        }

        private static void SetResolver(Assembly assembly)
        {
            NativeLibrary.SetDllImportResolver(assembly, (name, _, _) =>
                name == LibraryName ? NativeLibrary.Load(LibraryPath) : IntPtr.Zero);
        }

        private static string ResolvePath(string configured)
        {
            string path = Path.GetFullPath(configured);

            if (File.Exists(path))
            {
                return path;
            }

            if (Directory.Exists(path))
            {
                foreach (string candidate in new[] { "libpact_ffi.dylib", "libpact_ffi.so", "pact_ffi.dll" })
                {
                    string file = Path.Combine(path, candidate);
                    if (File.Exists(file))
                    {
                        return file;
                    }
                }
            }

            throw new FileNotFoundException($"{EnvironmentVariable}='{configured}' is not a pact_ffi library or a directory containing one", path);
        }
    }
}
