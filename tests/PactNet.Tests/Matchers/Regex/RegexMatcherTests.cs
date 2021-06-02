using FluentAssertions;
using Newtonsoft.Json;
using PactNet.Matchers.Regex;
using Xunit;

namespace PactNet.Tests.Matchers.Regex
{
    public class RegexMatcherTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            const string example = "hello@tester.com";
            const string regex = "^([\\w+\\-].?)+@[a-z\\d\\-]+(\\.[a-z]+)*\\.[a-z]+$";

            var matcher = new RegexMatcher(example, regex);

            string actual = JsonConvert.SerializeObject(matcher);
            string expected = $@"{{""pact:matcher:type"":""regex"",""value"":""{example}"",""regex"":{JsonConvert.SerializeObject(regex)}}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
