using PactNet.Models;
using Xunit;

namespace PactNet.Tests.Models
{
    public class InteractionTests
    {
        [Fact]
        public void ToString_WhenCalled_ReturnsJsonRepresentation()
        {
            const string expectedInteractionJson = "{\"description\":\"My description\",\"provider_state\":\"My provider state\"}";

            var interaction = new Interaction
            {
                Description = "My description",
                ProviderState = "My provider state"
            };

            var actualInteractionJson = interaction.ToString();

            Assert.Equal(expectedInteractionJson, actualInteractionJson);
        }
    }
}
