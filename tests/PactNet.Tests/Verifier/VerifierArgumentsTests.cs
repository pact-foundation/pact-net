using System;
using FluentAssertions;
using PactNet.Verifier;
using Xunit;

namespace PactNet.Tests.Verifier
{
    public class VerifierArgumentsTests
    {
        private readonly VerifierArguments args;

        public VerifierArgumentsTests()
        {
            this.args = new VerifierArguments();
        }

        [Fact]
        public void AddOption_WhenCalled_AddsOption()
        {
            string expected = string.Join(Environment.NewLine, "foo", "bar");

            this.args.AddOption("foo", "bar");

            this.args.ToString().Should().Be(expected);
        }

        [Fact]
        public void AddOption_RepeatedOption_AddsOptions()
        {
            string expected = string.Join(Environment.NewLine, "foo", "bar", "foo", "baz");

            this.args.AddOption("foo", "bar");
            this.args.AddOption("foo", "baz");

            this.args.ToString().Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddOption_InvalidValue_ThrowsArgumentException(string value)
        {
            Action action = () => this.args.AddOption("key", value, nameof(value));

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AddFlag_WhenCalled_AddsFlag()
        {
            this.args.AddFlag("foo");

            this.args.ToString().Should().Be("foo");
        }
    }
}
