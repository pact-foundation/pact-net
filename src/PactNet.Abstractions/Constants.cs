using System;
using System.IO;

namespace PactNet
{
    internal static class Constants
    {
#if USE_NET4X
        public static string BuildDirectory = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", ""))).LocalPath;
        public static string DefaultPactDir = Path.GetFullPath($"{BuildDirectory}{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}");
#else
        public static string BuildDirectory = AppContext.BaseDirectory;
        public static string DefaultPactDir = Path.GetFullPath($"{BuildDirectory}{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}");
#endif
    }
}
