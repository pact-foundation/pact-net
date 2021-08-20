using FluentAssertions;
using Newtonsoft.Json;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Tests.Matchers
{
    public class TypeMatcherTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            const string example = "hello@tester.com";

            var matcher = new TypeMatcher(example);

            string actual = JsonConvert.SerializeObject(matcher);
            string expected = $@"{{""messagePact:matcher:type"":""type"",""value"":""{example}""}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
