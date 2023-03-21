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
    /// Happy path integration tests to make sure wwe're calling the Rust FFI library properly with P/Invoke
    /// </summary>
    public class FfiIntegrationTests
    {
        private readonly ITestOutputHelper output;

        public FfiIntegrationTests(ITestOutputHelper output)
        {
            this.output = output;

            NativeInterop.LogToBuffer(LevelFilter.Trace);
        }

        [Fact]
        public async Task HttpInteraction_v3_CreatesPactFile()
        {
            var driver = new PactDriver();

            try
            {
                IHttpPactDriver pact = driver.NewHttpPact("NativeDriverTests-Consumer-V3",
                                                          "NativeDriverTests-Provider",
                                                          PactSpecification.V3);
            
                IHttpInteractionDriver interaction = pact.NewHttpInteraction("a sample interaction");

                interaction.Given("provider state");
                interaction.GivenWithParam("state with param", "foo", "bar");
                interaction.WithRequest("POST", "/path");
                //interaction.WithRequestHeader("X-Request-Header", "request1", 0);
                //interaction.WithRequestHeader("X-Request-Header", "request2", 1);
                interaction.WithQueryParameter("param", "value", 0);
                interaction.WithMultipartSingleFileUpload("multipart/form-data", "tests/PactNet.Tests/data/test_file.jpg", "boundary");

                //interaction.WithRequestBody("application/json", @"{""foo"":42}");

                interaction.WithResponseStatus((ushort)HttpStatusCode.Created);
                //interaction.WithResponseHeader("X-Response-Header", "value1", 0);
                //interaction.WithResponseHeader("X-Response-Header", "value2", 1);
                //interaction.WithResponseBody("application/json", @"{""foo"":42}");


                using IMockServerDriver mockServer = pact.CreateMockServer("127.0.0.1", null, false);

                var client = new HttpClient { BaseAddress = mockServer.Uri };
                client.DefaultRequestHeaders.Add("X-Request-Header", new[] { "request1", "request2" });

                HttpResponseMessage result = await client.PostAsync("/path?param=value", new StringContent(@"{""foo"":42}", Encoding.UTF8, "application/json"));
                //result.StatusCode.Should().Be(HttpStatusCode.Created);
                //result.Headers.GetValues("X-Response-Header").Should().BeEquivalentTo("value1", "value2");

                string content = await result.Content.ReadAsStringAsync();
                //content.Should().Be(@"{""foo"":42}");

                mockServer.MockServerMismatches().Should().Be("[]");

                string logs = mockServer.MockServerLogs();
                logs.Should().NotBeEmpty();

                this.output.WriteLine("Mock Server Logs");
                this.output.WriteLine("----------------");
                this.output.WriteLine(logs);

                pact.WritePactFile(Environment.CurrentDirectory);
            }
            finally
            {
                this.WriteDriverLogs(driver);
            }

            var file = new FileInfo("NativeDriverTests-Consumer-V3-NativeDriverTests-Provider.json");
            file.Exists.Should().BeTrue();

            string pactContents = File.ReadAllText(file.FullName).TrimEnd();
            string expectedPactContent = File.ReadAllText("data/v3-server-integration.json").TrimEnd();
            //pactContents.Should().Be(expectedPactContent);
        }

        private void WriteDriverLogs(IPactDriver pact)
        {
            this.output.WriteLine(string.Empty);
            this.output.WriteLine("Driver Logs");
            this.output.WriteLine("-----------");
            this.output.WriteLine(pact.DriverLogs());
        }

        [Fact]
        public void MessageInteraction_v3_CreatesPactFile()
        {
            var driver = new PactDriver();

            try
            {
                IMessagePactDriver pact = driver.NewMessagePact("NativeDriverTests-Consumer-V3",
                                                                "NativeDriverTests-Producer",
                                                                PactSpecification.V3);

                IMessageInteractionDriver interaction = pact.NewMessageInteraction("a message interaction");

                interaction.ExpectsToReceive("changed description");
                interaction.WithMetadata("foo", "bar");
                interaction.WithContents("application/json", @"{""foo"":42}", 0);

                string reified = interaction.Reify();
                reified.Should().NotBeNullOrEmpty();

                interaction.WritePactFile(Environment.CurrentDirectory);
            }
            finally
            {
                this.WriteDriverLogs(driver);
            }

            var file = new FileInfo("NativeDriverTests-Consumer-V3-NativeDriverTests-Producer.json");
            file.Exists.Should().BeTrue();

            string pactContents = File.ReadAllText(file.FullName).TrimEnd();
            string expectedPactContent = File.ReadAllText("data/v3-message-integration.json").TrimEnd();
            pactContents.Should().Be(expectedPactContent);
        }
    }
}
