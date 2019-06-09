using Xunit;
using PactNet.Extensions;

namespace PactNet.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ToLowerSnakeCase_ShouldReturnLowerSnakeCase()
        {
            var input = "Test String";

            Assert.Equal("test_string", input.ToLowerSnakeCase());
        }

        [Fact]
        public void ToLowerSnakeCase_ShouldReturnEmptyString_WhenInputIsNull()
        {
            Assert.Equal(string.Empty, ((string)null).ToLowerSnakeCase());
        }

        [Fact]
        public void EscapeDoubleQuotes_ShouldReplaceDoubleQuotesWithEscapedDoubleQuotes()
        {
            var input = "You say \"Goodbye\" and I say \"Hello\"";

            Assert.Equal("You say \\\"Goodbye\\\" and I say \\\"Hello\\\"", input.EscapeDoubleQuotes());
        }

        [Fact]
        public void EscapeDoubleQuotes_ShouldHandleStringWithoutQuotes()
        {
            var input = "No quotes";

            Assert.Equal("No quotes", input.EscapeDoubleQuotes());
        }

        [Fact]
        public void EscapeDoubleQuotes_ShouldReturn_EmptyStringWhenNull()
        {
            Assert.Equal(string.Empty, ((string)null).EscapeDoubleQuotes());
        }
    }
}
