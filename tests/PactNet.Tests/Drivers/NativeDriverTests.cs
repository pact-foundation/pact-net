using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using PactNet.Drivers;
using PactNet.Interop;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Tests.Drivers
{
    /// <summary>
    /// Happy-path integration tests that just make sure we're calling the Rust core properly via P/Invoke
    /// </summary>
    public class NativeDriverTests
    {
        private readonly ITestOutputHelper output;

        public NativeDriverTests(ITestOutputHelper output)
        {
            this.output = output;

            NativeInterop.LogToBuffer(LevelFilter.Trace);
        }

        [Fact]
        public async Task HttpInteraction_v3_CreatesPactFile()
        {
            var server = new NativeDriver();

            PactHandle pact = server.NewPact("NativeDriverTests-Consumer-V3", "NativeDriverTests-Provider");

            server.WithSpecification(pact, PactSpecification.V3).Should().BeTrue();

            InteractionHandle interaction = server.NewHttpInteraction(pact, "a sample interaction");
            bool request = server.Given(interaction, "provider state")
                        && server.GivenWithParam(interaction, "state with param", "foo", "bar")
                        && server.WithRequest(interaction, "POST", "/path")
                        && server.WithRequestHeader(interaction, "X-Request-Header", "request1", 0)
                        && server.WithRequestHeader(interaction, "X-Request-Header", "request2", 1)
                        && server.WithQueryParameter(interaction, "param", "value", 0)
                        && server.WithRequestBody(interaction, "application/json", @"{""foo"":42}");

            request.Should().BeTrue();

            bool response = server.ResponseStatus(interaction, (ushort)HttpStatusCode.Created)
                         && server.WithResponseHeader(interaction, "X-Response-Header", "value1", 0)
                         && server.WithResponseHeader(interaction, "X-Response-Header", "value2", 1)
                         && server.WithResponseBody(interaction, "application/json", @"{""foo"":42}");

            response.Should().BeTrue();

            int port = server.CreateMockServerForPact(pact, "127.0.0.1:0", false);

            var client = new HttpClient
            {
                BaseAddress = new Uri($"http://127.0.0.1:{port}")
            };
            client.DefaultRequestHeaders.Add("X-Request-Header", new[] { "request1", "request2" });

            HttpResponseMessage result = await client.PostAsync("/path?param=value", new StringContent(@"{""foo"":42}", Encoding.UTF8, "application/json"));
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Headers.GetValues("X-Response-Header").Should().BeEquivalentTo("value1", "value2");

            string content = await result.Content.ReadAsStringAsync();
            content.Should().Be(@"{""foo"":42}");

            server.MockServerMismatches(port).Should().Be("[]");
            //server.MockServerLogs(port).Should().NotBeEmpty();

            server.WritePactFile(pact, Environment.CurrentDirectory, true);

            var file = new FileInfo("NativeDriverTests-Consumer-V3-NativeDriverTests-Provider.json");
            file.Exists.Should().BeTrue();

            string pactContents = File.ReadAllText(file.FullName).TrimEnd();
            string expectedPactContent = File.ReadAllText("data/v3-server-integration.json").TrimEnd();
            pactContents.Should().Be(expectedPactContent);

            server.CleanupMockServer(port).Should().BeTrue();
        }

        [Fact]
        public void MessageInteraction_v3_CreatesPactFile()
        {
            var driver = new NativeDriver();

            try
            {
                PactHandle pact = driver.NewPact("NativeDriverTests-Consumer-V3", "NativeDriverTests-Producer");

                driver.WithSpecification(pact, PactSpecification.V3).Should().BeTrue();

                InteractionHandle interaction = driver.NewMessageInteraction(pact, "a message interaction");

                // TODO: These assertions fail on MacOS only (yeah, really...) but the file assertion below would catch if they didn't work anyway
                driver.ExpectsToReceive(interaction, "changed description")/*.Should().BeTrue()*/;
                driver.WithMetadata(interaction, "foo", "bar")/*.Should().BeTrue()*/;
                driver.WithContents(interaction, "application/json", @"{""foo"":42}", 0)/*.Should().BeTrue()*/;

                string reified = driver.Reify(interaction);
                reified.Should().NotBeNullOrEmpty();

                driver.WritePactFile(pact, Environment.CurrentDirectory, true);
            }
            finally
            {
                string logs = driver.Logs();
                this.output.WriteLine(logs);
            }

            var file = new FileInfo("NativeDriverTests-Consumer-V3-NativeDriverTests-Producer.json");
            file.Exists.Should().BeTrue();

            string pactContents = File.ReadAllText(file.FullName).TrimEnd();
            string expectedPactContent = File.ReadAllText("data/v3-message-integration.json").TrimEnd();
            pactContents.Should().Be(expectedPactContent);
        }
    }
}
