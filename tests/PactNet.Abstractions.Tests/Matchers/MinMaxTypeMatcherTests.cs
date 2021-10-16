using System;
using FluentAssertions;
using Newtonsoft.Json;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class MinMaxTypeMatcherTests
    {

        [Fact]
        public void Ctor_Min_SerialisesCorrectly()
        {
            var example = new[] { 22, 23, 56 };
            const int min = 2;

            var matcher = new MinMaxTypeMatcher(example, min);

            string actual = JsonConvert.SerializeObject(matcher);
            string expected = $@"{{""pact:matcher:type"":""type"",""value"":[22,23,56],""min"":{min}}}";

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Ctor_Max_SerialisesCorrectly()
        {
            var example = new[] { 22, 23, 56 };
            const int max = 2;

            var matcher = new MinMaxTypeMatcher(example, max: max);

            string actual = JsonConvert.SerializeObject(matcher);
            string expected = $@"{{""pact:matcher:type"":""type"",""value"":[22,23,56],""max"":{max}}}";

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Ctor_MinMax_SerialisesCorrectly()
        {
            var example = new[] { 22, 23, 56 };
            const int min = 1;
            const int max = 2;

            var matcher = new MinMaxTypeMatcher(example, min, max);

            string actual = JsonConvert.SerializeObject(matcher);
            string expected = $@"{{""pact:matcher:type"":""type"",""value"":[22,23,56],""min"":{min},""max"":{max}}}";

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Ctor_WhenCalledWithMin0_ThrowsArgumentException()
        {
            var example = new[] { 22, 23, 56 };
            const int min = 0;

            Action action = () => new MinMaxTypeMatcher(example, min);

            action.Should().Throw<ArgumentException>();
        }
    }
}
