using System;
using System.Collections.Generic;
using FluentAssertions;
using PactNet.Native.Internal;
using Xunit;

namespace PactNet.Native.Tests.Internal
{
    public class DictionaryExtensionsTests
    {
        private readonly IDictionary<string, string> args;

        public DictionaryExtensionsTests()
        {
            this.args = new Dictionary<string, string>();
        }

        [Fact]
        public void AddOption_WhenCalled_AddsOption()
        {
            this.args.AddOption("foo", "bar");

            this.args.Should().Contain("foo", "bar");
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

            this.args.Should().Contain("foo", string.Empty);
        }
    }
}
