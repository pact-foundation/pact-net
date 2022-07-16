using FluentAssertions;
using Newtonsoft.Json;
using PactNet.Generators;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Generators
{
    public class ProviderStateGeneratorTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            const string example = "hello@tester.com";
            const string expression = "${email}";

            var generator = new ProviderStateGenerator(example, expression);

            string actual = JsonConvert.SerializeObject(generator);
            string expected = $@"{{""pact:matcher:type"":""type"",""value"":""{example}"",""pact:generator:type"":""ProviderState"",""expression"":""{expression}""}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
