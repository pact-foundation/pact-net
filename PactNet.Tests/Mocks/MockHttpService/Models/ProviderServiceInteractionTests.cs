using System.Collections.Generic;
using Nancy;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Models
{
    public class ProviderServiceInteractionTests
    {
        [Fact]
        public void ToString_WhenCalled_ReturnsJsonRepresentation()
        {
            const string expectedInteractionJson = "{\"description\":\"My description\",\"provider_state\":\"My provider state\",\"request\":{\"method\":\"delete\",\"path\":\"/tester\",\"query\":\"test=2\",\"headers\":{\"Accept\":\"application/json\"},\"body\":{\"test\":\"hello\"}},\"response\":{\"status\":407,\"headers\":{\"Content-Type\":\"application/json\"},\"body\":{\"yep\":\"it worked\"}}}";
            var interaction = new ProviderServiceInteraction
            {
                Request = new ProviderServiceRequest
                {
                    Method = HttpVerb.Delete,
                    Body = new
                    {
                        test = "hello"
                    },
                    Headers = new Dictionary<string, string>
                    {
                        { "Accept", "application/json" }
                    },
                    Path = "/tester",
                    Query = "test=2"
                },
                Response = new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.ProxyAuthenticationRequired,
                    Body = new
                    {
                        yep = "it worked"
                    },
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    }
                },
                Description = "My description",
                ProviderState = "My provider state",
            };

            var actualInteractionJson = interaction.ToString();

            Assert.Equal(expectedInteractionJson, actualInteractionJson);
        }
    }
}
