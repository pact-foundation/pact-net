using System;
using FluentAssertions;
using PactNet.Internal;
using Xunit;

namespace PactNet.Tests.Internal
{
    public class GuardTests
    {
        [Fact]
        public void NotNullOrEmpty_Success_DoesNotThrow()
        {
            Action action = () => Guard.NotNullOrEmpty("foo", "bar");

            action.Should().NotThrow();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void NotNullorEmpty_Failure_ThrowsArgumentException(string input)
        {
            Action action = () => Guard.NotNullOrEmpty(input, "name");

            action.Should().Throw<ArgumentException>().Which.ParamName.Should().Be("name");
        }

        [Fact]
        public void NotNull_Success_DoesNotThrow()
        {
            Action action = () => Guard.NotNull("foo", "bar");

            action.Should().NotThrow();
        }

        [Fact]
        public void NotNull_Failure_ThrowsArgumentException()
        {
            Action action = () => Guard.NotNull(null, "name");

            action.Should().Throw<ArgumentException>().Which.ParamName.Should().Be("name");
        }
    }
}
