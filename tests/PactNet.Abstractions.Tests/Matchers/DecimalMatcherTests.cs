using System.Text.Json;
using FluentAssertions;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class DecimalMatcherTests
    {
        [Fact]
        public void Ctor_Float_SerialisesCorrectly()
        {
            const float example = 3.14f;

            var matcher = new DecimalMatcher(example);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = $@"{{""pact:matcher:type"":""decimal"",""value"":{JsonSerializer.Serialize(example)}}}";

            actual.Should().Be(expected);
        }

        [Fact]
        public void Ctor_Double_SerialisesCorrectly()
        {
            const double example = 3.14;

            var matcher = new DecimalMatcher(example);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = $@"{{""pact:matcher:type"":""decimal"",""value"":{JsonSerializer.Serialize(example)}}}";

            actual.Should().Be(expected);
        }

        [Fact]
        public void Ctor_Decimal_SerialisesCorrectly()
        {
            const decimal example = 3.1m;

            var matcher = new DecimalMatcher(example);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = $@"{{""pact:matcher:type"":""decimal"",""value"":{JsonSerializer.Serialize(example)}}}";

            actual.Should().Be(expected);
        }
    }
}
