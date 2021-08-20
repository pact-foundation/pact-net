using FluentAssertions;
using Newtonsoft.Json;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Tests.Matchers
{
    public class NullMatcherTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            var matcher = new NullMatcher();

            string actual = JsonConvert.SerializeObject(matcher);
            string expected = $@"{{""messagePact:matcher:type"":""null""}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
