using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Matchers.Type;
using Xunit;

namespace PactNet.Tests.Matchers.Type
{
    public class TypeMatcherTests
    {
        private TypeMatcher GetSubject(dynamic example)
        {
            return new TypeMatcher(example);
        }

        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            const string example = "hello@tester.com";

            var matcher = GetSubject(example);

            var expected = new
            {
                json_class = "Pact::SomethingLike",
                contents = example
            };
            var expectedJson = JsonConvert.SerializeObject(expected, JsonConfig.ApiSerializerSettings);
            var actualJson = JsonConvert.SerializeObject(matcher, JsonConfig.ApiSerializerSettings);

            Assert.Equal(expectedJson, actualJson);
        }
    }
}