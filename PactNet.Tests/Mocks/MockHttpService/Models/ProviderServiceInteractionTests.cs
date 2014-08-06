using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Models
{
    public class ProviderServiceInteractionTests
    {
        [Fact]
        public void IncrementUsage_WhenCalled_SetsUsageCountToOne()
        {
            var interaction = new ProviderServiceInteraction();

            interaction.IncrementUsage();

            Assert.Equal(1, interaction.UsageCount);
        }

        [Fact]
        public void IncrementUsage_WhenCalledTwice_SetsUsageCountToTwo()
        {
            var interaction = new ProviderServiceInteraction();

            interaction.IncrementUsage();
            interaction.IncrementUsage();

            Assert.Equal(2, interaction.UsageCount);
        }

        [Fact]
        public void UsageCount_WhenIncrementUsageIsNotExecuted_DefaultsToZero()
        {
            var interaction = new ProviderServiceInteraction();

            Assert.Equal(0, interaction.UsageCount);
        }
    }
}
