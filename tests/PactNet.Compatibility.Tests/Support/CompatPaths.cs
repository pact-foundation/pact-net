using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PactNet.Compatibility.Tests.Support
{
    /// <summary>
    /// Locations baked in at build time, so fixtures and the baseline are read in place from the source tree
    /// </summary>
    internal static class CompatPaths
    {
        public static string SuiteDir { get; } = Metadata("CompatSuiteDir");

        public static string ProjectDir { get; } = Metadata("CompatProjectDir");

        public static string FixturesDir => Path.Combine(SuiteDir, "fixtures");

        public static string BaselineFile =>
            Environment.GetEnvironmentVariable("COMPAT_BASELINE_FILE") is { Length: > 0 } custom
                ? custom
                : Path.Combine(ProjectDir, "baseline.yaml");

        public static string Fixture(string name)
        {
            string path = Path.Combine(FixturesDir, name);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Fixture '{name}' not found in {FixturesDir} (is the pact-compatibility-suite submodule checked out?)", path);
            }

            return path;
        }

        private static string Metadata(string key) =>
            typeof(CompatPaths).Assembly
                               .GetCustomAttributes<AssemblyMetadataAttribute>()
                               .Single(a => a.Key == key)
                               .Value;
    }
}
