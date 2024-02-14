using System.Text.Json;
using FluentAssertions;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class MatcherConverterTests
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        [Fact]
        public void Serialise_Decimal_SerialisesCorrectly()
        {
            IMatcher matcher = Match.Decimal(42.1m);

            string actual = JsonSerializer.Serialize(matcher, Options);

            actual.Should().Be(@"{""pact:matcher:type"":""decimal"",""value"":42.1}");
        }

        [Fact]
        public void Serialise_Equality_SerialisesCorrectly()
        {
            IMatcher matcher = Match.Equality("foo");

            string actual = JsonSerializer.Serialize(matcher, Options);

            actual.Should().Be(@"{""pact:matcher:type"":""equality"",""value"":""foo""}");
        }

        [Fact]
        public void Serialise_Include_SerialisesCorrectly()
        {
            IMatcher matcher = Match.Include("foo");

            string actual = JsonSerializer.Serialize(matcher, Options);

            actual.Should().Be(@"{""pact:matcher:type"":""include"",""value"":""foo""}");
        }

        [Fact]
        public void Serialise_Integer_SerialisesCorrectly()
        {
            IMatcher matcher = Match.Integer(42);

            string actual = JsonSerializer.Serialize(matcher, Options);

            actual.Should().Be(@"{""pact:matcher:type"":""integer"",""value"":42}");
        }

        [Fact]
        public void Serialise_MinMax_SerialisesCorrectly()
        {
            IMatcher matcher = Match.MinMaxType("foo", 1, 2);

            string actual = JsonSerializer.Serialize(matcher, Options);

            actual.Should().Be(@"{""pact:matcher:type"":""type"",""value"":[""foo""],""min"":1,""max"":2}");
        }

        [Fact]
        public void Serialise_Null_SerialisesCorrectly()
        {
            IMatcher matcher = Match.Null();

            string actual = JsonSerializer.Serialize(matcher, Options);

            actual.Should().Be(@"{""pact:matcher:type"":""null""}");
        }

        [Fact]
        public void Serialise_Numeric_SerialisesCorrectly()
        {
            IMatcher matcher = Match.Number(42);

            string actual = JsonSerializer.Serialize(matcher, Options);

            actual.Should().Be(@"{""pact:matcher:type"":""number"",""value"":42}");
        }

        [Fact]
        public void Serialise_Regex_SerialisesCorrectly()
        {
            IMatcher matcher = Match.Regex("foo", "^foo$");

            string actual = JsonSerializer.Serialize(matcher, Options);

            actual.Should().Be(@"{""pact:matcher:type"":""regex"",""value"":""foo"",""regex"":""^foo$""}");
        }

        [Fact]
        public void Serialise_Type_SerialisesCorrectly()
        {
            IMatcher matcher = Match.Type("foo");

            string actual = JsonSerializer.Serialize(matcher, Options);

            actual.Should().Be(@"{""pact:matcher:type"":""type"",""value"":""foo""}");
        }
    }
}
