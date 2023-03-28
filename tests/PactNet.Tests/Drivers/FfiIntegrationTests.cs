using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
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
        public async Task HttpInteraction_v3_CreatesPactFile_WithMultiPartRequest()
        {
            var driver = new PactDriver();

            try
            {
                IHttpPactDriver pact = driver.NewHttpPact("NativeDriverTests-Consumer-V3",
                                                          "NativeDriverTests-Provider-Multipart",
                                                          PactSpecification.V4);

                IHttpInteractionDriver interaction = pact.NewHttpInteraction("a sample interaction");

                string contentType = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "application/octet-stream" : "image/jpeg";

                interaction.Given("provider state");
                interaction.WithRequest("POST", "/path");
                var path = Path.GetFullPath("data/test_file.jpeg");

                var fileInfo = new FileInfo(path);
                Assert.True(File.Exists(fileInfo.FullName));

                interaction.WithFileUpload(contentType, fileInfo.FullName, "file");

                interaction.WithResponseStatus((ushort)HttpStatusCode.Created);
                interaction.WithResponseHeader("X-Response-Header", "value1", 0);
                interaction.WithResponseHeader("X-Response-Header", "value2", 1);
                interaction.WithResponseBody("application/json", @"{""foo"":42}");

                using IMockServerDriver mockServer = pact.CreateMockServer("127.0.0.1", null, false);

                var client = new HttpClient { BaseAddress = mockServer.Uri };

                using var fileStream = new FileStream("data/test_file.jpeg", FileMode.Open, FileAccess.Read);

                var upload = new MultipartFormDataContent();
                upload.Headers.ContentType.MediaType = "multipart/form-data";

                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

                var fileName = Path.GetFileName(path);
                var fileNameBytes = Encoding.UTF8.GetBytes(fileName);
                var encodedFileName = Convert.ToBase64String(fileNameBytes);
                upload.Add(fileContent, "file", fileName);
                upload.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = fileName,
                    FileNameStar = $"utf-8''{encodedFileName}"
                };

                HttpResponseMessage result = await client.PostAsync("/path", upload);
                result.StatusCode.Should().Be(HttpStatusCode.Created);

                string logs = mockServer.MockServerLogs();

                string content = await result.Content.ReadAsStringAsync();
                content.Should().Be(@"{""foo"":42}");

                mockServer.MockServerMismatches().Should().Be("[]");

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
            // The body and boundry will be different, so test the header and matching rules are multipart/form-data
            var file = new FileInfo("NativeDriverTests-Consumer-V3-NativeDriverTests-Provider-Multipart.json");
            file.Exists.Should().BeTrue();

            string pactContents = File.ReadAllText(file.FullName).TrimEnd();
            JObject pactObject = JObject.Parse(pactContents);

            string expectedPactContent = File.ReadAllText("data/v3-server-integration-MultipartFormDataBody.json").TrimEnd();
            JObject expectedPactObject = JObject.Parse(pactContents);


            string contentTypeHeader = (string)pactObject["interactions"][0]["request"]["headers"]["Content-Type"][0];
            Assert.Contains("multipart/form-data;", contentTypeHeader);


            JArray integrationsArray = (JArray)pactObject["interactions"];
            JToken matchingRules = integrationsArray.First["request"]["matchingRules"];

            JArray expecteIntegrationsArray = (JArray)expectedPactObject["interactions"];
            JToken expectedMatchingRules = expecteIntegrationsArray.First["request"]["matchingRules"];

            Assert.True(JToken.DeepEquals(matchingRules, expectedMatchingRules));              
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
                interaction.WithRequestHeader("X-Request-Header", "request1", 0);
                interaction.WithRequestHeader("X-Request-Header", "request2", 1);
                interaction.WithQueryParameter("param", "value", 0);
                interaction.WithRequestBody("application/json", @"{""foo"":42}");

                interaction.WithResponseStatus((ushort)HttpStatusCode.Created);
                interaction.WithResponseHeader("X-Response-Header", "value1", 0);
                interaction.WithResponseHeader("X-Response-Header", "value2", 1);
                interaction.WithResponseBody("application/json", @"{""foo"":42}");

                using IMockServerDriver mockServer = pact.CreateMockServer("127.0.0.1", null, false);

                var client = new HttpClient { BaseAddress = mockServer.Uri };
                client.DefaultRequestHeaders.Add("X-Request-Header", new[] { "request1", "request2" });

                HttpResponseMessage result = await client.PostAsync("/path?param=value", new StringContent(@"{""foo"":42}", Encoding.UTF8, "application/json"));
                result.StatusCode.Should().Be(HttpStatusCode.Created);
                result.Headers.GetValues("X-Response-Header").Should().BeEquivalentTo("value1", "value2");

                string content = await result.Content.ReadAsStringAsync();
                content.Should().Be(@"{""foo"":42}");

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
            pactContents.Should().Be(expectedPactContent);
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
