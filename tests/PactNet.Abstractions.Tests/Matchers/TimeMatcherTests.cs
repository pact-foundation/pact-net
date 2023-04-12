using System;
using System.Globalization;
using FluentAssertions;
using FluentAssertions.Extensions;
using Newtonsoft.Json;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class TimeMatcherTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            DateTime example = 14.February(2023).At(11, 12, 13);

            var matcher = new TimeMatcher(example, "HH:mm:ss");

            string actual = JsonConvert.SerializeObject(matcher, new JsonSerializerSettings() { Culture = CultureInfo.InvariantCulture });
            string expected = $@"{{""pact:matcher:type"":""time"",""value"":""11:12:13"",""format"":""HH:mm:ss""}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
