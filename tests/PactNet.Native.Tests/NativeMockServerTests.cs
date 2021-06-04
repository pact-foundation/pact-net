using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace PactNet.Native.Tests
{
    /// <summary>
    /// Happy-path integration tests that just make sure we're calling the Rust core properly via P/Invoke
    /// </summary>
    public class NativeMockServerTests
    {
        [Fact]
        public async Task HappyPathIntegrationTest()
        {
            var server = new NativeMockServer();

            PactHandle pact = server.NewPact("NativeMockServerTests-Consumer-V3", "NativeMockServerTests-Provider");

            server.WithSpecification(pact, PactSpecification.V3).Should().BeTrue();

            InteractionHandle interaction = server.NewInteraction(pact, "a sample interaction");
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

            server.WritePactFile(port, Environment.CurrentDirectory, true);

            var file = new FileInfo("NativeMockServerTests-Consumer-V3-NativeMockServerTests-Provider.json");
            file.Exists.Should().BeTrue();

            string pactContents = File.ReadAllText(file.FullName).TrimEnd();
            string expectedPactContent = File.ReadAllText("data/v3-server-integration.json").TrimEnd();
            pactContents.Should().Be(expectedPactContent);

            server.CleanupMockServer(port).Should().BeTrue();
        }
    }
}
