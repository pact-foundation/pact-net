using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using PactNet.Compatibility.Tests.Support;
using Reqnroll;

namespace PactNet.Compatibility.Tests.Steps
{
    /// <summary>
    /// Steps for the HTTP consumer features (written against features/V1/http_consumer.feature)
    /// </summary>
    [Binding]
    public sealed class HttpConsumerSteps
    {
        private static readonly string[] Ordinals =
            { "first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eighth", "ninth", "tenth" };

        private readonly CompatWorld world;

        public HttpConsumerSteps(CompatWorld world)
        {
            this.world = world;
        }

        [Given(@"^the following HTTP interactions have been defined:$")]
        public void InteractionsDefined(Table table) => this.world.Run("interactions defined", () =>
        {
            this.world.Interactions = InteractionDefinition.FromTable(table);
        });

        [When(@"^the mock server is started with interactions? ""?([\d, ]+)""?$")]
        public Task MockServerStarted(string numbers) => this.world.RunAsync("start mock server", async () =>
        {
            List<InteractionDefinition> selected = numbers.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                                          .Select(n => this.world.Interaction(int.Parse(n)))
                                                          .ToList();

            this.world.Session = await MockServerSession.StartAsync(selected);
        });

        [When(@"^request (\d+) is made to the mock server$")]
        public Task RequestMade(int no) => this.world.RunAsync($"request {no}", async () =>
        {
            this.world.Response = await this.world.RequireSession().SendAsync(this.world.Interaction(no));
        });

        [When(@"^request (\d+) is made to the mock server with the following changes:$")]
        public Task RequestMadeWithChanges(int no, Table changes) => this.world.RunAsync($"request {no} with changes", async () =>
        {
            InteractionDefinition request = this.world.Interaction(no).WithChanges(changes);
            this.world.Response = await this.world.RequireSession().SendAsync(request);
        });

        [Then(@"^a (\d+) (?:success|error) response is returned$")]
        public void ResponseReturned(int status) => this.world.Run("response status", () =>
        {
            HttpResponse response = this.RequireResponse();
            CompatWorld.Check(response.Status == status,
                $"Expected a {status} response but got {response.Status}: {Encoding.UTF8.GetString(response.Body)}");
        });

        [Then(@"^the payload will contain the ""([^""]*)"" JSON document$")]
        public void PayloadJson(string name) => this.world.Run("payload JSON document", () =>
        {
            JsonNode expected = JsonNode.Parse(File.ReadAllText(CompatPaths.Fixture(name + ".json")));
            JsonNode actual = JsonNode.Parse(this.RequireResponse().Body);
            CompatWorld.Check(JsonNode.DeepEquals(expected, actual), $"Expected payload {expected?.ToJsonString()} but got {actual?.ToJsonString()}");
        });

        [Then(@"^the content type will be set as ""([^""]*)""$")]
        public void ContentType(string contentType) => this.world.Run("response content type", () =>
        {
            string actual = this.RequireResponse().ContentType;
            CompatWorld.Check(MediaTypeEquals(actual, contentType), $"Expected content type '{contentType}' but got '{actual}'");
        });

        [When(@"^the pact test is done$")]
        public Task PactTestDone() => this.world.RunAsync("pact test done", () => this.world.RequireSession().FinishAsync());

        [Then(@"^the mock server status will be OK$")]
        public void StatusOk() => this.world.Run("status OK", () =>
        {
            MockServerSession session = this.RequireDone();
            CompatWorld.Check(session.Ok, $"Expected the mock server status to be OK, but verification failed: {session.VerifyError?.Message}\n{session.FinalMismatches?.ToJsonString()}");
        });

        [Then(@"^the mock server status will NOT be OK$")]
        public void StatusNotOk() => this.world.Run("status NOT OK", () =>
        {
            CompatWorld.Check(!this.RequireDone().Ok, "Expected the mock server status to NOT be OK, but verification passed");
        });

        [Then(@"^the mock server will write out a Pact file for the interactions? when done$")]
        public void PactWritten() => this.world.Run("pact file written", () =>
        {
            MockServerSession session = this.RequireDone();
            CompatWorld.Check(File.Exists(session.PactFile), $"Expected a pact file at {session.PactFile}");
        });

        [Then(@"^the mock server will NOT write out a Pact file for the interactions? when done$")]
        public void PactNotWritten() => this.world.Run("pact file not written", () =>
        {
            MockServerSession session = this.RequireDone();
            CompatWorld.Check(!File.Exists(session.PactFile), $"Expected no pact file, but found {session.PactFile}");
        });

        [Then(@"^the pact file will contain \{(\d+)\} interactions?$")]
        public void PactInteractionCount(int count) => this.world.Run("pact interaction count", () =>
        {
            JsonArray interactions = this.PactInteractions();
            CompatWorld.Check(interactions.Count == count, $"Expected {count} interaction(s) in the pact file but found {interactions.Count}");
        });

        [Then(@"^the \{(\w+)\} interaction request will be for a ""([^""]*)""$")]
        public void InteractionMethod(string ordinal, string method) => this.world.Run("pact request method", () =>
        {
            string actual = (string)this.PactInteraction(ordinal)["request"]?["method"];
            CompatWorld.Check(string.Equals(actual, method, StringComparison.OrdinalIgnoreCase), $"Expected request method '{method}' but the pact has '{actual}'");
        });

        [Then(@"^the \{(\w+)\} interaction request query parameters will be ""([^""]*)""$")]
        public void InteractionQuery(string ordinal, string query) => this.world.Run("pact request query", () =>
        {
            JsonNode actual = this.PactInteraction(ordinal)["request"]?["query"];
            var expected = InteractionDefinition.ParseQuery(query);
            var actualPairs = actual switch
            {
                null => new List<KeyValuePair<string, string>>(),
                JsonValue v => InteractionDefinition.ParseQuery((string)v),
                JsonObject o => o.SelectMany(p => p.Value is JsonArray a
                                                      ? a.Select(x => new KeyValuePair<string, string>(p.Key, (string)x))
                                                      : new[] { new KeyValuePair<string, string>(p.Key, (string)p.Value) }).ToList(),
                _ => throw new CompatAssertionException($"Unexpected query representation {actual.ToJsonString()}")
            };

            CompatWorld.Check(expected.OrderBy(p => p.Key).SequenceEqual(actualPairs.OrderBy(p => p.Key)),
                $"Expected query '{query}' but the pact has {actual?.ToJsonString()}");
        });

        [Then(@"^the \{(\w+)\} interaction request will contain the header ""([^""]*)"" with value ""([^""]*)""$")]
        public void InteractionHeader(string ordinal, string name, string value) => this.world.Run("pact request header", () =>
        {
            JsonNode headers = this.PactInteraction(ordinal)["request"]?["headers"];
            string actual = HeaderValue(headers, name);
            CompatWorld.Check(actual == value, $"Expected header '{name}: {value}' but the pact has {headers?.ToJsonString()}");
        });

        [Then(@"^the \{(\w+)\} interaction request content type will be ""([^""]*)""$")]
        public void InteractionContentType(string ordinal, string contentType) => this.world.Run("pact request content type", () =>
        {
            JsonNode request = this.PactInteraction(ordinal)["request"];
            string actual = HeaderValue(request?["headers"], "Content-Type") ?? (string)request?["body"]?["contentType"];
            CompatWorld.Check(MediaTypeEquals(actual, contentType), $"Expected request content type '{contentType}' but the pact has '{actual}'");
        });

        [Then(@"^the \{(\w+)\} interaction (request|response) will contain the ""([^""]*)"" document$")]
        public void InteractionDocument(string ordinal, string part, string name) => this.world.Run($"pact {part} document", () =>
        {
            BodyDefinition expected = BodyDefinition.FromFixture(name);
            JsonNode actual = this.PactInteraction(ordinal)[part]?["body"];

            // V4 pacts wrap the body as {content, contentType, encoded}
            if (actual is JsonObject wrapper && wrapper.ContainsKey("content") && wrapper.ContainsKey("contentType"))
            {
                actual = wrapper["content"];
            }

            if (expected.ContentType.Contains("json", StringComparison.OrdinalIgnoreCase))
            {
                JsonNode expectedJson = JsonNode.Parse(expected.Bytes);
                JsonNode actualJson = actual is JsonValue v && v.TryGetValue(out string text) ? JsonNode.Parse(text) : actual;
                CompatWorld.Check(JsonNode.DeepEquals(expectedJson, actualJson),
                    $"Expected {part} body {expectedJson?.ToJsonString()} but the pact has {actual?.ToJsonString()}");
            }
            else
            {
                string actualText = actual is JsonValue v && v.TryGetValue(out string s) ? s : actual?.ToJsonString();
                CompatWorld.Check(actualText == expected.Text, $"Expected {part} body '{expected.Text}' but the pact has '{actualText}'");
            }
        });

        [Then(@"^the mock server status will be an expected but not received error for interaction \{(\d+)\}$")]
        public void ExpectedNotReceived(int no) => this.world.Run("missing request mismatch", () =>
        {
            InteractionDefinition interaction = this.world.Interaction(no);
            JsonArray mismatches = this.RequireDone().FinalMismatches;

            CompatWorld.Check(mismatches.Any(m => (string)m?["type"] == "missing-request" && RequestMatches(m, interaction.Method, interaction.Path)),
                $"Expected a missing-request mismatch for {interaction.Method} {interaction.Path}, got {mismatches.ToJsonString()}");
        });

        [Then(@"^the mock server status will be an unexpected ""([^""]*)"" request received error for interaction \{(\d+)\}$")]
        public void UnexpectedRequestForInteraction(string method, int no) => this.world.Run("unexpected request mismatch", () =>
        {
            InteractionDefinition interaction = this.world.Interaction(no);
            JsonArray mismatches = this.RequireDone().FinalMismatches;

            CompatWorld.Check(mismatches.Any(m => (string)m?["type"] == "request-not-found" && RequestMatches(m, method, interaction.Path)),
                $"Expected a request-not-found mismatch for {method} {interaction.Path}, got {mismatches.ToJsonString()}");
        });

        [Then(@"^the mock server status will be an unexpected ""([^""]*)"" request received error for path ""([^""]*)""$")]
        public void UnexpectedRequestForPath(string method, string path) => this.world.Run("unexpected request mismatch", () =>
        {
            JsonArray mismatches = this.RequireDone().FinalMismatches;

            CompatWorld.Check(mismatches.Any(m => (string)m?["type"] == "request-not-found" && RequestMatches(m, method, path)),
                $"Expected a request-not-found mismatch for {method} {path}, got {mismatches.ToJsonString()}");
        });

        [Then(@"^the mock server status will be mismatches$")]
        public void StatusMismatches() => this.world.Run("request mismatches", () =>
        {
            MockServerSession session = this.RequireDone();
            CompatWorld.Check(!session.Ok && session.FinalMismatches.Any(m => (string)m?["type"] == "request-mismatch"),
                $"Expected request mismatches, got {session.FinalMismatches?.ToJsonString()}");
        });

        [Then(@"^the mismatches will contain a ""([^""]*)"" mismatch with error ""(.*)""$")]
        public void MismatchWithError(string kind, string error) => this.world.Run($"{kind} mismatch", () =>
        {
            this.AssertMismatch(kind, null, error);
        });

        [Then(@"^the mismatches will contain a ""([^""]*)"" mismatch with path ""([^""]*)"" with error ""(.*)""$")]
        public void MismatchWithPathAndError(string kind, string path, string error) => this.world.Run($"{kind} mismatch", () =>
        {
            this.AssertMismatch(kind, path, error);
        });

        private void AssertMismatch(string kind, string path, string error)
        {
            JsonArray mismatches = this.world.RequireSession().LiveMismatches();

            IEnumerable<JsonNode> details = mismatches.SelectMany(m => m?["mismatches"] as JsonArray ?? new JsonArray());

            bool found = details.Any(d =>
            {
                string type = ((string)d?["type"] ?? string.Empty).ToLowerInvariant();
                bool typeMatches = kind == "body-content-type"
                                       ? type == "bodytypemismatch"
                                       : type.StartsWith(kind.Replace("-", string.Empty).ToLowerInvariant(), StringComparison.Ordinal);

                if (!typeMatches || !((string)d["mismatch"] ?? string.Empty).Contains(error, StringComparison.Ordinal))
                {
                    return false;
                }

                if (path == null)
                {
                    return true;
                }

                string location = (string)d["path"] ?? (string)d["key"] ?? (string)d["parameter"];
                return location == path;
            });

            CompatWorld.Check(found, $"Expected a '{kind}' mismatch{(path == null ? string.Empty : $" at '{path}'")} with error \"{error}\", got {mismatches.ToJsonString()}");
        }

        private static bool RequestMatches(JsonNode mismatch, string method, string path)
        {
            string actualMethod = (string)mismatch["method"] ?? (string)mismatch["request"]?["method"];
            string actualPath = (string)mismatch["path"] ?? (string)mismatch["request"]?["path"];
            return string.Equals(actualMethod, method, StringComparison.OrdinalIgnoreCase) && actualPath == path;
        }

        private static string HeaderValue(JsonNode headers, string name)
        {
            if (headers is not JsonObject obj)
            {
                return null;
            }

            foreach (KeyValuePair<string, JsonNode> header in obj)
            {
                if (header.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return header.Value is JsonArray values ? string.Join(", ", values.Select(v => (string)v)) : (string)header.Value;
                }
            }

            return null;
        }

        private static bool MediaTypeEquals(string actual, string expected)
        {
            static string Strip(string ct) => ct?.Split(';')[0].Trim();
            return string.Equals(Strip(actual), Strip(expected), StringComparison.OrdinalIgnoreCase);
        }

        private HttpResponse RequireResponse() =>
            this.world.Response ?? throw new InvalidOperationException("No request has been made to the mock server");

        private MockServerSession RequireDone()
        {
            MockServerSession session = this.world.RequireSession();
            return session.IsDone ? session : throw new InvalidOperationException("The pact test has not been marked as done");
        }

        private JsonArray PactInteractions()
        {
            MockServerSession session = this.RequireDone();
            CompatWorld.Check(File.Exists(session.PactFile), $"No pact file was written to {session.PactFile}");

            JsonNode pact = JsonNode.Parse(File.ReadAllText(session.PactFile));
            return pact?["interactions"] as JsonArray ?? new JsonArray();
        }

        private JsonNode PactInteraction(string ordinal)
        {
            int index = Array.IndexOf(Ordinals, ordinal.ToLowerInvariant());
            if (index < 0)
            {
                throw new NotSupportedException($"Unknown ordinal '{ordinal}'");
            }

            JsonArray interactions = this.PactInteractions();
            CompatWorld.Check(index < interactions.Count, $"The pact file has {interactions.Count} interaction(s), no {ordinal} interaction");
            return interactions[index];
        }
    }
}
