using System.Globalization;
using System.Text.Json;
using FluentAssertions;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class NumericMatcherTests
    {
        [Fact]
        public void Ctor_Int_SerialisesCorrectly()
        {
            const int example = 42;

            var matcher = new NumericMatcher(example);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = $@"{{""pact:matcher:type"":""number"",""value"":{JsonSerializer.Serialize(example)}}}";

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Ctor_Float_SerialisesCorrectly()
        {
            const float example = 3.14f;

            var matcher = new NumericMatcher(example);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = $@"{{""pact:matcher:type"":""number"",""value"":{JsonSerializer.Serialize(example)}}}";

            actual.Should().Be(expected);
        }

        [Fact]
        public void Ctor_Double_SerialisesCorrectly()
        {
            const double example = 3.14;

            var matcher = new NumericMatcher(example);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = $@"{{""pact:matcher:type"":""number"",""value"":{JsonSerializer.Serialize(example)}}}";

            actual.Should().Be(expected);
        }

        [Fact]
        public void Ctor_Numeric_SerialisesCorrectly()
        {
            const decimal example = 3.14m;

            var matcher = new NumericMatcher(example);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = $@"{{""pact:matcher:type"":""number"",""value"":{JsonSerializer.Serialize(example)}}}";

            actual.Should().Be(expected.ToString(CultureInfo.InvariantCulture));
        }
    }
}
