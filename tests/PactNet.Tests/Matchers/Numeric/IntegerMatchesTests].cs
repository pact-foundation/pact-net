using FluentAssertions;
using Newtonsoft.Json;
using PactNet.Matchers.Numeric;
using Xunit;

namespace PactNet.Tests.Matchers.Numeric
{
    public class IntegerMatcherTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            const int example = 42;

            var matcher = new IntegerMatcher(example);

            string actual = JsonConvert.SerializeObject(matcher);
            string expected = $@"{{""pact:matcher:type"":""integer"",""value"":{example}}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
