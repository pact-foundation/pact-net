using System.Text.Json;
using FluentAssertions;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class ArrayContainsMatcherTests
    {
        [Fact]
        public void Ctor_String_SerializesCorrectly()
        {
            // Arrange
            var example = new[]
            {
                "Thing1",
                "Thing2",
            };

            var matcher = new ArrayContainsMatcher(example);

            // Act
            var actual = JsonSerializer.Serialize(matcher);

            // Assert
            actual.Should().Be(@"{""pact:matcher:type"":""array-contains"",""variants"":[""Thing1"",""Thing2""]}");
        }
    }
}
