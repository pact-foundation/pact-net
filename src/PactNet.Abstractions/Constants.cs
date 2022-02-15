using System;
using System.IO;

namespace PactNet
{
    internal static class Constants
    {
        public static string BuildDirectory = AppContext.BaseDirectory;
        public static string DefaultPactDir = Path.GetFullPath($"{BuildDirectory}{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}");
    }
}
