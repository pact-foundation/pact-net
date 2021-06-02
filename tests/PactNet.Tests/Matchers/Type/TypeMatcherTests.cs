using FluentAssertions;
using Newtonsoft.Json;
using PactNet.Matchers.Type;
using Xunit;

namespace PactNet.Tests.Matchers.Type
{
    public class TypeMatcherTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            const string example = "hello@tester.com";

            var matcher = new TypeMatcher(example);

            string actual = JsonConvert.SerializeObject(matcher);
            string expected = $@"{{""pact:matcher:type"":""type"",""value"":""{example}""}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
