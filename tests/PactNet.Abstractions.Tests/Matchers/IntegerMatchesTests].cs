using System.Text.Json;
using FluentAssertions;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class IntegerMatcherTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            const int example = 42;

            var matcher = new IntegerMatcher(example);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = $@"{{""pact:matcher:type"":""integer"",""value"":{example}}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
