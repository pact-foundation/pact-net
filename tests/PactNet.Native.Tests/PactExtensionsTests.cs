using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Matchers;
using PactNet.Native;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Native.Tests
{
    public class PactExtensionsTests
    {
        private readonly ITestOutputHelper output;

        public PactExtensionsTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task ConsumerIntegrationTest()
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            IPact pact = new Pact("PactExtensionsTests-Consumer", "PactExtensionsTests-Provider", "2.0.0", new PactConfig
            {
                DefaultJsonSettings = jsonSettings,
                Outputters = new[]
                {
                    new XUnitOutput(this.output)
                },
                PactDir = Environment.CurrentDirectory
            });

            IPactBuilder builder = pact.UsingNativeBackend();

            builder.UponReceiving("a sample request")
                   .Given("a provider state")
                   .WithRequest(HttpMethod.Post, "/things")
                   .WithHeader("X-Request", "request1")
                   .WithHeader("X-Request", "request2")
                   .WithQuery("param", "value1")
                   .WithQuery("param", "value2")
                   .WithJsonBody(new
                   {
                       Bool = Match.Type(true),
                       Int = Match.Type(42),
                       String = Match.Type("foo")
                   })
                   .WillRespond()
                   .WithStatus(HttpStatusCode.Created)
                   .WithHeader("X-Response", "response1")
                   .WithHeader("X-Response", "response2")
                   .WithJsonBody(new TestData
                   {
                       Bool = true,
                       Int = 42,
                       String = "foo"
                   });

            using (IPactContext context = builder.Build())
            {
                await PerformRequest(context, new TestData
                {
                    Bool = true,
                    Int = 42,
                    String = "foo"
                }, jsonSettings);
            }

            string actualPact = File.ReadAllText("PactExtensionsTests-Consumer-PactExtensionsTests-Provider.json").TrimEnd();
            string expectedPact = File.ReadAllText("data/v2-consumer-integration.json").TrimEnd();

            actualPact.Should().Be(expectedPact);
        }

        private async Task PerformRequest(IPactContext context, TestData body, JsonSerializerSettings jsonSettings)
        {
            var client = new HttpClient
            {
                BaseAddress = context.MockServerUri
            };
            client.DefaultRequestHeaders.Add("X-Request", new[] { "request1", "request2" });

            string content = JsonConvert.SerializeObject(body, jsonSettings);

            HttpResponseMessage response = await client.PostAsync("/things?param=value1&param=value2",
                                                                  new StringContent(content, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.GetValues("X-Response").Should().BeEquivalentTo(new[] { "response1", "response2" });

            string responseContent = await response.Content.ReadAsStringAsync();
            TestData responseData = JsonConvert.DeserializeObject<TestData>(responseContent, jsonSettings);
            responseData.Should().BeEquivalentTo(body);
        }

        public class TestData
        {
            public bool Bool { get; set; }
            public int Int { get; set; }
            public string String { get; set; }
        }
    }
}
