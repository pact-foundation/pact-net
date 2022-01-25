using FluentAssertions;
using Xunit;

namespace PactNet.Abstractions.Tests
{
    public class PactConfigTests
    {
        [Fact]
        public void Ctor_WithDefaults_UsesDefaultPactDir()
        {
            var options = new PactConfig();

            options.PactDir.Should().Be(Constants.DefaultPactDir);
        }
    }
}
