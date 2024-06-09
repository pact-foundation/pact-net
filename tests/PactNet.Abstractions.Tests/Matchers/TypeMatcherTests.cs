using System.Text.Json;
using FluentAssertions;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class TypeMatcherTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            const string example = "hello@tester.com";

            var matcher = new TypeMatcher(example);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = $@"{{""pact:matcher:type"":""type"",""value"":""{example}""}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
