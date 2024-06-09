using System;
using System.Text.Json;
using FluentAssertions;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class MinMaxTypeMatcherTests
    {

        [Fact]
        public void Ctor_Min_SerialisesCorrectly()
        {
            var example = 42;
            const int min = 2;

            var matcher = new MinMaxTypeMatcher(example, min);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = @"{""pact:matcher:type"":""type"",""value"":[42],""min"":2}";

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Ctor_Max_SerialisesCorrectly()
        {
            var example = 42;
            const int max = 2;

            var matcher = new MinMaxTypeMatcher(example, max: max);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = @"{""pact:matcher:type"":""type"",""value"":[42],""max"":2}";

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Ctor_MinMax_SerialisesCorrectly()
        {
            var example = 42;
            const int min = 1;
            const int max = 2;

            var matcher = new MinMaxTypeMatcher(example, min, max);

            string actual = JsonSerializer.Serialize(matcher);
            string expected = @"{""pact:matcher:type"":""type"",""value"":[42],""min"":1,""max"":2}";

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Ctor_WhenCalledWithMin0_ThrowsArgumentException()
        {
            var example = 42;
            const int min = 0;

            Action action = () => new MinMaxTypeMatcher(example, min);

            action.Should().Throw<ArgumentException>();
        }
    }
}
