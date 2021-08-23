using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PactNet;
using PactNet.Native;
using Xunit;
using Xunit.Abstractions;

namespace ReadMe.Consumer.Tests
{
    public class SomethingApiConsumerTests
    {
        private readonly IPactBuilderV3 PactBuilder;

        public SomethingApiConsumerTests(ITestOutputHelper output)
        {
            // Use default pact directory ..\..\pacts and default log
            // directory ..\..\logs
            var pact = Pact.V3("Something API Consumer", "Something API", new PactConfig());

            // or specify custom log and pact directories
            pact = Pact.V3("Something API Consumer", "Something API", new PactConfig
            {
                PactDir = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName}{Path.DirectorySeparatorChar}pacts",
                LogDir = @"c:\temp\logs",
            });

            // Initialize Rust backend
            PactBuilder = pact.UsingNativeBackend();
        }

        [Fact]
        public async Task GetSomething_WhenTheTesterSomethingExists_ReturnsTheSomething()
        {
            // Arrange
            PactBuilder
                .UponReceiving("A GET request to retrieve the something")
                    .Given("There is a something with id 'tester'")
                    .WithRequest(HttpMethod.Get, "/somethings/tester")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new
                    {
                        // NOTE: These properties are case sensitive!
                        id = "tester",
                        firstName = "Totally",
                        lastName = "Awesome"
                    });

            await PactBuilder.VerifyAsync(async ctx =>
            {
                // Act
                var client = new SomethingApiClient(ctx.MockServerUri);
                var something = await client.GetSomething("tester");

                // Assert
                Assert.Equal("tester", something.Id);
            });
        }
    }
}
