using System.Text.Json;
using FluentAssertions;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class EqualityMatcherTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            const string example = "example";

            var matcher = new EqualityMatcher(example);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = $@"{{""pact:matcher:type"":""equality"",""value"":""{example}""}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
