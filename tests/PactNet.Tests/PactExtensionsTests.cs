using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using PactNet.Matchers;
using PactNet.Output.Xunit;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS0618

namespace PactNet.Tests
{
    public class PactExtensionsTests
    {
        private readonly TestData example = new()
        {
            Bool = true,
            Int = 42,
            String = "foo",
            Children = new[]
            {
                new TestData
                {
                    Bool = false,
                    Int = 7,
                    String = "bar"
                }
            }
        };

        private readonly dynamic matcher = new
        {
            Bool = Match.Type(true),
            Int = Match.Type(42),
            String = Match.Type("foo"),
            Children = Match.MinType(new
            {
                Bool = Match.Type(false),
                Int = Match.Type(7),
                String = Match.Type("bar")
            }, 1)
        };

        private readonly PactConfig config;

        public PactExtensionsTests(ITestOutputHelper output)
        {
            var jsonSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            this.config = new PactConfig
            {
                PactDir = Environment.CurrentDirectory,
                DefaultJsonSettings = jsonSettings,
                Outputters = new[]
                {
                    new XunitOutput(output)
                }
            };

            File.Delete("PactExtensionsTests-Consumer-V2-PactExtensionsTests-Provider.json");
            File.Delete("PactExtensionsTests-Consumer-V3-PactExtensionsTests-Provider.json");
            File.Delete("PactExtensionsTests-Consumer-V4-PactExtensionsTests-Provider.json");
            File.Delete("PactExtensionsTests-Combined-V4-PactExtensionsTests-Provider.json");
            File.Delete("PactExtensioñsTests-Combined-V4-UTF8-PactExtensioñsTests-Provider-UTF8.json");
        }

        [Fact]
        public async Task WithHttpInteractions_V2_CreatesExpectedPactFile()
        {
            IPactV2 pact = Pact.V2("PactExtensionsTests-Consumer-V2", "PactExtensionsTests-Provider", this.config);
            IPactBuilderV2 builder = pact.WithHttpInteractions();

            builder.UponReceiving("a sample request")
                       .Given("a provider state")
                       .WithRequest(HttpMethod.Post, "/things")
                       .WithHeader("X-Request", "request1")
                       .WithHeader("X-Request", "request2")
                       .WithQuery("param", "value1")
                       .WithQuery("param", "value2")
                       .WithJsonBody(this.matcher)
                   .WillRespond()
                       .WithStatus(HttpStatusCode.Created)
                       .WithHeader("X-Response", "response1")
                       .WithHeader("X-Response", "response2")
                       .WithJsonBody(this.example);

            await builder.VerifyAsync(async ctx =>
            {
                await PerformRequestAsync(ctx, this.example, this.config.DefaultJsonSettings);
            });

            string actualPact = File.ReadAllText("PactExtensionsTests-Consumer-V2-PactExtensionsTests-Provider.json").TrimEnd();
            string expectedPact = File.ReadAllText("data/v2-consumer-integration.json").TrimEnd();

            actualPact.Should().Be(expectedPact);
        }

        [Fact]
        public async Task WithHttpInteractions_V3_CreatesExpectedPactFile()
        {
            IPactV3 pact = Pact.V3("PactExtensionsTests-Consumer-V3", "PactExtensionsTests-Provider", this.config);
            IPactBuilderV3 builder = pact.WithHttpInteractions();

            builder.UponReceiving("a sample request")
                       .Given("a provider state")
                       .Given("another provider state")
                       .Given("a provider state with params", new Dictionary<string, string>
                       {
                           ["foo"] = "bar",
                           ["baz"] = "bash"
                       })
                       .WithRequest(HttpMethod.Post, "/things")
                       .WithHeader("X-Request", "request1")
                       .WithHeader("X-Request", "request2")
                       .WithQuery("param", "value1")
                       .WithQuery("param", "value2")
                       .WithJsonBody(this.matcher)
                   .WillRespond()
                       .WithStatus(HttpStatusCode.Created)
                       .WithHeader("X-Response", "response1")
                       .WithHeader("X-Response", "response2")
                       .WithJsonBody(this.example);

            await builder.VerifyAsync(async ctx =>
            {
                await PerformRequestAsync(ctx, this.example, this.config.DefaultJsonSettings);
            });

            string actualPact = File.ReadAllText("PactExtensionsTests-Consumer-V3-PactExtensionsTests-Provider.json").TrimEnd();
            string expectedPact = File.ReadAllText("data/v3-consumer-integration.json").TrimEnd();

            actualPact.Should().Be(expectedPact);
        }

        [Fact]
        public async Task WithHttpInteractions_V4_CreatesExpectedPactFile()
        {
            IPactV4 pact = Pact.V4("PactExtensionsTests-Consumer-V4", "PactExtensionsTests-Provider", this.config);
            IPactBuilderV4 builder = pact.WithHttpInteractions();

            builder.UponReceiving("a sample request")
                       .Given("a provider state")
                       .Given("another provider state")
                       .Given("a provider state with params", new Dictionary<string, string>
                        {
                            ["foo"] = "bar",
                            ["baz"] = "bash"
                        })
                       .WithRequest(HttpMethod.Post, "/things")
                       .WithHeader("X-Request", "request1")
                       .WithHeader("X-Request", "request2")
                       .WithQuery("param", "value1")
                       .WithQuery("param", "value2")
                       .WithJsonBody(this.matcher)
                   .WillRespond()
                       .WithStatus(HttpStatusCode.Created)
                       .WithHeader("X-Response", "response1")
                       .WithHeader("X-Response", "response2")
                       .WithJsonBody(this.example);

            await builder.VerifyAsync(async ctx =>
            {
                await PerformRequestAsync(ctx, this.example, this.config.DefaultJsonSettings);
            });

            string actualPact = File.ReadAllText("PactExtensionsTests-Consumer-V4-PactExtensionsTests-Provider.json").TrimEnd();
            string expectedPact = File.ReadAllText("data/v4-consumer-integration.json").TrimEnd();

            actualPact.Should().Be(expectedPact);
        }

        [Fact]
        public void WithMessageInteractions_V3_CreatesExpectedPactFile()
        {
            IPactV3 messagePact = Pact.V3("PactExtensionsTests-MessageConsumer-V3", "PactExtensionsTests-MessageProvider", config);
            IMessagePactBuilderV3 builder = messagePact.WithMessageInteractions();

            builder
                .WithPactMetadata("framework", "language", "C#")
                .ExpectsToReceive("a sample request")
                .Given("a provider state")
                .Given("another provider state")
                .Given("a provider state with params", new Dictionary<string, string>
                {
                    ["foo"] = "bar",
                    ["baz"] = "bash"
                })
                .WithMetadata("queueId", "1234")
                .WithJsonContent(new TestData { Int = 1, String = "a description" })
                .Verify<TestData>(_ => { });

            string actualPact = File.ReadAllText("PactExtensionsTests-MessageConsumer-V3-PactExtensionsTests-MessageProvider.json").TrimEnd();
            string expectedPact = File.ReadAllText("data/v3-message-consumer-integration.json").TrimEnd();

            actualPact.Should().Be(expectedPact);
        }

        [Fact]
        public void WithMessageInteractions_V4_CreatesExpectedPactFile()
        {
            IPactV4 messagePact = Pact.V4("PactExtensionsTests-MessageConsumer-V4", "PactExtensionsTests-MessageProvider", config);
            IMessagePactBuilderV4 builder = messagePact.WithMessageInteractions();

            builder
               .WithPactMetadata("framework", "language", "C#")
               .ExpectsToReceive("a sample request")
               .Given("a provider state")
               .Given("another provider state")
               .Given("a provider state with params", new Dictionary<string, string>
                {
                    ["foo"] = "bar",
                    ["baz"] = "bash"
                })
               .WithMetadata("queueId", "1234")
               .WithJsonContent(new TestData { Int = 1, String = "a description" })
               .Verify<TestData>(_ => { });

            string actualPact = File.ReadAllText("PactExtensionsTests-MessageConsumer-V4-PactExtensionsTests-MessageProvider.json").TrimEnd();
            string expectedPact = File.ReadAllText("data/v4-message-consumer-integration.json").TrimEnd();

            actualPact.Should().Be(expectedPact);
        }

        [Fact]
        public async Task CombinedHttpAndMessageInteractions_v4_CreatesExpectedPactFile()
        {
            IPactV4 pact = Pact.V4("PactExtensionsTests-Combined-V4", "PactExtensionsTests-Provider", config);

            // http interaction
            IPactBuilderV4 http = pact.WithHttpInteractions();

            http.UponReceiving("a HTTP request")
                    .Given("a provider state")
                    .Given("another provider state")
                    .Given("a provider state with params", new Dictionary<string, string>
                    {
                        ["foo"] = "bar",
                        ["baz"] = "bash"
                    })
                    .WithRequest(HttpMethod.Post, "/things")
                    .WithHeader("X-Request", "request1")
                    .WithHeader("X-Request", "request2")
                    .WithQuery("param", "value1")
                    .WithQuery("param", "value2")
                    .WithJsonBody(this.matcher)
                .WillRespond()
                    .WithStatus(HttpStatusCode.Created)
                    .WithHeader("X-Response", "response1")
                    .WithHeader("X-Response", "response2")
                    .WithJsonBody(this.example);

            await http.VerifyAsync(async ctx =>
            {
                await PerformRequestAsync(ctx, this.example, this.config.DefaultJsonSettings);
            });

            // message interaction
            IMessagePactBuilderV4 message = pact.WithMessageInteractions();

            message
                .WithPactMetadata("framework", "language", "C#")
                .ExpectsToReceive("a message")
                .Given("a provider state")
                .Given("another provider state")
                .Given("a provider state with params", new Dictionary<string, string>
                {
                    ["foo"] = "bar",
                    ["baz"] = "bash"
                })
                .WithMetadata("queueId", "1234")
                .WithJsonContent(new TestData { Int = 1, String = "a description" })
                .Verify<TestData>(_ => { });

            string actualPact = File.ReadAllText("PactExtensionsTests-Combined-V4-PactExtensionsTests-Provider.json").TrimEnd();
            string expectedPact = File.ReadAllText("data/v4-combined-integration.json").TrimEnd();

            actualPact.Should().Be(expectedPact);
        }

#if NET7_0_OR_GREATER
        [Fact]
        public async Task CombinedHttpAndMessageInteractions_v4_HandlesNonAsciiCharactersInUserInput()
        {
            IPactV4 pact = Pact.V4("PactExtensioñsTests-Combined-V4-UTF8", "PactExtensioñsTests-Provider-UTF8", config);

            // http interaction
            IPactBuilderV4 http = pact.WithHttpInteractions();

            http.UponReceiving("a HTTP request with non-ASCII characters like ñ")
                    .Given("a provider state with ñ")
                    .Given("another provider state with ñ")
                    .Given("a provider state with params with ñ", new Dictionary<string, string>
                    {
                        ["foo"] = "bañ",
                        ["bañ"] = "bash"
                    })
                    .WithRequest(HttpMethod.Post, "/things")
                    .WithJsonBody(new
                    {
                        foo = Match.Type("ñ request")
                    })
                .WillRespond()
                    .WithStatus(HttpStatusCode.Created)
                    .WithJsonBody(new
                    {
                        Foo = "ñ response"
                    });

            await http.VerifyAsync(async ctx =>
            {
                var client = new HttpClient
                {
                    BaseAddress = ctx.MockServerUri
                };
                string content = JsonSerializer.Serialize(new { Foo = "ñ request" }, this.config.DefaultJsonSettings);
                HttpResponseMessage response = await client.PostAsync("/things", new StringContent(content, Encoding.UTF8, "application/json"));
                string responseContent = await response.Content.ReadAsStringAsync();
                responseContent.Should().Be(@"{""foo"":""ñ response""}");
                response.StatusCode.Should().Be(HttpStatusCode.Created);
            });

            // message interaction
            IMessagePactBuilderV4 message = pact.WithMessageInteractions();

            message
                .WithPactMetadata("framework", "language", "C#")
                .ExpectsToReceive("a message with ñ")
                .Given("a provider state with ñ")
                .Given("another provider state with ñ")
                .Given("a provider state with params with ñ", new Dictionary<string, string>
                {
                    ["foo"] = "bañ",
                    ["bañ"] = "bash"
                })
                .WithMetadata("queueId", "1234ñ")
                .WithJsonContent(new TestData { Int = 1, String = "a description with ñ" })
                .Verify<TestData>(_ => { });

            string actualPact = File.ReadAllText("PactExtensioñsTests-Combined-V4-UTF8-PactExtensioñsTests-Provider-UTF8.json").TrimEnd();
            string expectedPact = File.ReadAllText("data/v4-non-ascii-integration.json").TrimEnd();

            actualPact.Should().Be(expectedPact);
        }
#endif

        private static async Task PerformRequestAsync(IConsumerContext context, TestData body, JsonSerializerOptions jsonSettings)
        {
            var client = new HttpClient
            {
                BaseAddress = context.MockServerUri
            };
            client.DefaultRequestHeaders.Add("X-Request", new[] { "request1", "request2" });

            string content = JsonSerializer.Serialize(body, jsonSettings);

            HttpResponseMessage response = await client.PostAsync("/things?param=value1&param=value2",
                                                                  new StringContent(content, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.GetValues("X-Response").Should().BeEquivalentTo(new[] { "response1", "response2" });

            string responseContent = await response.Content.ReadAsStringAsync();
            TestData responseData = JsonSerializer.Deserialize<TestData>(responseContent, jsonSettings);
            responseData.Should().BeEquivalentTo(body);
        }

        public class TestData
        {
            public bool Bool { get; set; }
            public int Int { get; set; }
            public string String { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public ICollection<TestData> Children { get; set; }
        }
    }
}
