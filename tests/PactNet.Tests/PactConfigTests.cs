using FluentAssertions;
using Xunit;

namespace PactNet.Tests
{
    public class PactConfigTests
    {
        [Fact]
        public void Ctor_WithDefaults_UsesDefaultPactDir()
        {
            var options = new PactConfig();

            options.PactDir.Should().Be(Constants.DefaultPactDir);
        }

        [Fact]
        public void Ctor_WithDefaults_UsesDefaultLogDir()
        {
            var options = new PactConfig();

            options.LogDir.Should().Be(Constants.DefaultLogDir);
        }
    }
}
