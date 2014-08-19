using PactNet.Models;
using Xunit;

namespace PactNet.Tests.Models
{
    public class InteractionTests
    {
        [Fact]
        public void Summary_WhenCalledWithDescriptionAndProviderState_ReturnsSummaryWithDescriptionAndProviderState()
        {
            var interaction = new Interaction
            {
                Description = "My description",
                ProviderState = "My provider state"
            };

            var summary = interaction.Summary();

            Assert.Equal(interaction.Description + " - " + interaction.ProviderState, summary);
        }

        [Fact]
        public void Summary_WhenCalledWithOnlyDescription_ReturnsSummaryWithDescription()
        {
            var interaction = new Interaction
            {
                Description = "My description"
            };

            var summary = interaction.Summary();

            Assert.Equal(interaction.Description, summary);
        }
    }
}
