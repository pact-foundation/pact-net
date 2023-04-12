using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PactNet;
using PactNet.Matchers;
using Xunit;

namespace ReadMe.Consumer.Tests
{
    public class SomethingApiConsumerTests
    {
        private readonly IPactBuilderV4 pactBuilder;

        public SomethingApiConsumerTests()
        {
            // Use default pact directory ..\..\pacts and default log
            // directory ..\..\logs
            var pact = Pact.V4("Something API Consumer", "Something API", new PactConfig());

            // or specify custom log and pact directories
            pact = Pact.V4("Something API Consumer", "Something API", new PactConfig
            {
                PactDir = "../../../pacts/"
            });

            // Initialize Rust backend
            this.pactBuilder = pact.WithHttpInteractions();
        }

        [Fact]
        public async Task GetSomething_WhenTheTesterSomethingExists_ReturnsTheSomething()
        {
            // Arrange
            this.pactBuilder
                .UponReceiving("A GET request to retrieve the something")
                    .Given("There is a something with id 'tester'")
                    .WithRequest(HttpMethod.Get, "/somethings/tester")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new
                    {
                        id = Match.Type("tester"),
                        firstName = Match.Type("Totally"),
                        lastName = Match.Type("Awesome")
                    });

            await this.pactBuilder.VerifyAsync(async ctx =>
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
