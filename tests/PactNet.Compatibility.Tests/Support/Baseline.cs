using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PactNet.Compatibility.Tests.Support
{
    /// <summary>
    /// One scenario that is known not to pass
    /// </summary>
    public sealed class BaselineEntry
    {
        public string Feature { get; set; }

        public string Scenario { get; set; }

        /// <summary>
        /// failing | unsupported
        /// </summary>
        public string Status { get; set; }

        public string Reason { get; set; }
    }

    /// <summary>
    /// The ratchet baseline: scenarios expected not to pass, keyed by feature file and scenario name
    /// </summary>
    public sealed class Baseline
    {
        private const string Header =
            "# Compatibility suite baseline (ratchet) for pact-net.\n" +
            "# Lists scenarios that are NOT expected to pass, with a reason.\n" +
            "# A run fails if an unlisted scenario fails, or a listed scenario now passes.\n" +
            "# Regenerate with COMPAT_UPDATE_BASELINE=1 (existing reasons are kept).\n";

        private readonly Dictionary<(string, string), BaselineEntry> entries;

        private Baseline(IEnumerable<BaselineEntry> entries)
        {
            this.entries = entries.ToDictionary(e => (e.Feature, e.Scenario));
        }

        public IReadOnlyCollection<BaselineEntry> Entries => this.entries.Values;

        public static Baseline Load(string path)
        {
            if (!File.Exists(path))
            {
                return new Baseline(Enumerable.Empty<BaselineEntry>());
            }

            IDeserializer deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            List<BaselineEntry> list = deserializer.Deserialize<List<BaselineEntry>>(File.ReadAllText(path)) ?? new List<BaselineEntry>();
            return new Baseline(list);
        }

        public BaselineEntry Find(string feature, string scenario) =>
            this.entries.TryGetValue((feature, scenario), out BaselineEntry entry) ? entry : null;

        public static void Write(string path, IEnumerable<BaselineEntry> entries)
        {
            List<BaselineEntry> ordered = entries.OrderBy(e => e.Feature, StringComparer.Ordinal)
                                                 .ThenBy(e => e.Scenario, StringComparer.Ordinal)
                                                 .ToList();

            var sb = new StringBuilder(Header);

            if (ordered.Count == 0)
            {
                sb.AppendLine("[]");
            }
            else
            {
                ISerializer serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
                sb.Append(serializer.Serialize(ordered));
            }

            File.WriteAllText(path, sb.ToString());
        }
    }
}
