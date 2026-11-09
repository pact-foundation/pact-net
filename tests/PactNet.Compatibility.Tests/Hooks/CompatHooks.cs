using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PactNet.Compatibility.Tests.Support;
using Reqnroll;
using Reqnroll.UnitTestProvider;

namespace PactNet.Compatibility.Tests.Hooks
{
    /// <summary>
    /// Native library override, per-scenario cleanup and the baseline ratchet
    /// </summary>
    [Binding]
    public sealed class CompatHooks
    {
        private static readonly ConcurrentDictionary<(string Feature, string Scenario), (string Outcome, string Detail)> Results = new();

        private static Baseline baseline;

        private static bool UpdateMode => Environment.GetEnvironmentVariable("COMPAT_UPDATE_BASELINE") == "1";

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            NativeLibraryOverride.Install();
            baseline = Baseline.Load(CompatPaths.BaselineFile);
        }

        [AfterScenario(Order = 0)]
        public static async Task CleanUp(CompatWorld world)
        {
            if (world.Session != null)
            {
                await world.Session.DisposeAsync();
                world.Session = null;
            }
        }

        [AfterScenario(Order = 100)]
        public static void Ratchet(CompatWorld world, FeatureContext feature, ScenarioContext scenario, IUnitTestRuntimeProvider runtime)
        {
            string featureKey = FeatureKey(feature.FeatureInfo);
            string scenarioKey = scenario.ScenarioInfo.Title;

            ScenarioExecutionStatus status = scenario.ScenarioExecutionStatus;
            string outcome;
            string detail;

            // Reqnroll itself skips scenarios with undefined/pending steps; calling TestIgnore again would turn that into a failure
            bool reqnrollSkips = status is ScenarioExecutionStatus.UndefinedStep or ScenarioExecutionStatus.StepDefinitionPending;

            if (reqnrollSkips || world.Unsupported != null)
            {
                outcome = "unsupported";
                detail = world.Unsupported?.Message ?? $"undefined step(s) ({status})";
            }
            else if (world.Failure != null || scenario.TestError != null)
            {
                outcome = "failing";
                Exception error = world.Failure ?? scenario.TestError;
                detail = $"step '{world.FailedStep}': {error.GetType().Name}: {error.Message}";
            }
            else
            {
                outcome = "passed";
                detail = null;
            }

            Results[(featureKey, scenarioKey)] = (outcome, detail);

            if (UpdateMode)
            {
                if (!reqnrollSkips && outcome != "passed")
                {
                    runtime.TestIgnore($"[{outcome}] {detail}");
                }

                return;
            }

            BaselineEntry entry = baseline.Find(featureKey, scenarioKey);

            switch (outcome)
            {
                case "unsupported" when !reqnrollSkips:
                    // tolerated: reported as skipped, never fails the run
                    runtime.TestIgnore($"Unsupported: {detail}");
                    break;

                case "unsupported":
                    // undefined/pending steps: Reqnroll already marks the test as skipped (missingOrPendingStepsOutcome = Ignore)
                    break;

                case "failing" when entry == null:
                    throw new CompatAssertionException(
                        $"Scenario failed and is not in the baseline ({featureKey}: {scenarioKey})\n{detail}");

                case "failing":
                    runtime.TestIgnore($"Baselined ({entry.Status}): {entry.Reason}\nActual: {detail}");
                    break;

                case "passed" when entry != null:
                    throw new CompatAssertionException(
                        $"Scenario now passes but is listed in the baseline ({featureKey}: {scenarioKey}); remove it from baseline.yaml or rerun with COMPAT_UPDATE_BASELINE=1");
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            var counts = Results.Values.GroupBy(r => r.Outcome).ToDictionary(g => g.Key, g => g.Count());
            var summary = new StringBuilder();
            summary.AppendLine($"pact_ffi: {NativeLibraryOverride.LibraryPath ?? "(bundled)"}");
            summary.AppendLine($"scenarios: {Results.Count}");

            foreach (string key in new[] { "passed", "failing", "unsupported" })
            {
                summary.AppendLine($"{key}: {counts.GetValueOrDefault(key)}");
            }

            foreach (var result in Results.Where(r => r.Value.Outcome == "failing").OrderBy(r => r.Key.Feature).ThenBy(r => r.Key.Scenario))
            {
                summary.AppendLine($"FAILING {result.Key.Feature}: {result.Key.Scenario} -- {result.Value.Detail}");
            }

            File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "compat-summary.txt"), summary.ToString());

            if (UpdateMode)
            {
                WriteBaseline();
            }
        }

        private static void WriteBaseline()
        {
            var entries = new List<BaselineEntry>();

            // scenarios not executed in this run (e.g. filtered out) keep their entries
            entries.AddRange(baseline.Entries.Where(e => !Results.ContainsKey((e.Feature, e.Scenario))));

            foreach (var result in Results.Where(r => r.Value.Outcome == "failing"))
            {
                BaselineEntry existing = baseline.Find(result.Key.Feature, result.Key.Scenario);
                entries.Add(new BaselineEntry
                {
                    Feature = result.Key.Feature,
                    Scenario = result.Key.Scenario,
                    Status = "failing",
                    Reason = existing?.Reason ?? $"not yet triaged: {result.Value.Detail}"
                });
            }

            Baseline.Write(CompatPaths.BaselineFile, entries);
        }

        private static string FeatureKey(FeatureInfo info)
        {
            string folder = (info.FolderPath ?? string.Empty).Replace('\\', '/').Trim('/');
            // e.g. "pact-compatibility-suite/features/V1" -> "V1"
            int features = folder.LastIndexOf("features", StringComparison.OrdinalIgnoreCase);
            if (features >= 0)
            {
                folder = folder.Substring(features + "features".Length).Trim('/');
            }

            return string.IsNullOrEmpty(folder) ? info.Title : $"{folder}/{info.Title}";
        }
    }
}
