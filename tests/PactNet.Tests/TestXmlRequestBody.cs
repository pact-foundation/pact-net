using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PactNet.Tests
{
    public class TestXmlRequestBody
    {
        [Theory]
        [InlineData("AnyText", "text/plain")]
        [InlineData(@"{ ""simple"":""json"" }", "application/json")]
        [InlineData("<?xml version='1.0'?><rootNode><rootNode/>", "application/xml")]
        public async Task Post_simple_request_content(string body, string contentType)
        {
            // Arrange: Define simple POST interaction
            var pactBuilder = Pact.V3("consumer", "provider").WithHttpInteractions();           
            pactBuilder
                .UponReceiving("any description")
                .Given("any given")
                    .WithRequest(HttpMethod.Post, "/any/endpoint")
                    .WithBody(body, contentType)
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK);

            await pactBuilder.VerifyAsync(async ctx =>
            {
                // Act: Make POST request with the same body and content type
                var client = new HttpClient();
                client.BaseAddress = ctx.MockServerUri;
                var something = await client.PostAsync("/any/endpoint", new StringContent(body, Encoding.UTF8, contentType));

                // Assert
                Assert.True(something.IsSuccessStatusCode);
            });
        }
    }
}
