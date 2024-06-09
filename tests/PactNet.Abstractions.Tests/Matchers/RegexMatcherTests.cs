using System.Text.Json;
using FluentAssertions;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class RegexMatcherTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            const string example = "hello@tester.com";
            const string regex = "^([\\w+\\-].?)+@[a-z\\d\\-]+(\\.[a-z]+)*\\.[a-z]+$";

            var matcher = new RegexMatcher(example, regex);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = $@"{{""pact:matcher:type"":""regex"",""value"":""{example}"",""regex"":{JsonSerializer.Serialize(regex)}}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
