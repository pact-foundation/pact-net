using System;
using System.Globalization;
using FluentAssertions;
using FluentAssertions.Extensions;
using Newtonsoft.Json;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class DateMatcherTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            DateTime example = 14.February(2023);

            var matcher = new DateMatcher(example, "yyyy-MM-dd");

            string actual = JsonConvert.SerializeObject(matcher, new JsonSerializerSettings() { Culture = CultureInfo.InvariantCulture });
            string expected = $@"{{""pact:matcher:type"":""date"",""value"":""2023-02-14"",""format"":""yyyy-MM-dd""}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
