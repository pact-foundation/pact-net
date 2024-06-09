using System.Text.Json;
using FluentAssertions;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class NullMatcherTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            var matcher = new NullMatcher();

            string actual = JsonSerializer.Serialize(matcher);
            string expected = $@"{{""pact:matcher:type"":""null""}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
