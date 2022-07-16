using FluentAssertions;
using PactNet.Generators;
using Xunit;

namespace PactNet.Abstractions.Tests.Generators
{
    public class GenerateTests
    {
        [Fact]
        public void ProviderState_WhenCalled_ReturnsGenerator()
        {
            const string example = "/ticket/WO1FN";
            const string expression = @"/ticket/${pnr}";

            var matcher = Generate.ProviderState(example, expression);

            matcher.Should().BeEquivalentTo(new ProviderStateGenerator(example, expression));
        }
    }
}
