using System;
using System.Globalization;
using FluentAssertions;
using FluentAssertions.Extensions;
using Newtonsoft.Json;
using PactNet.Matchers;
using Xunit;

namespace PactNet.Abstractions.Tests.Matchers
{
    public class DateTimeMatcherTests
    {
        [Fact]
        public void Ctor_WhenCalled_SerialisesCorrectly()
        {
            DateTime example = 14.February(2023).At(11, 12, 13);

            var matcher = new DateTimeMatcher(example, "yyyy-MM-dd HH:mm:ss");

            string actual = JsonConvert.SerializeObject(matcher, new JsonSerializerSettings() { Culture = CultureInfo.InvariantCulture });
            string expected = $@"{{""pact:matcher:type"":""timestamp"",""value"":""2023-02-14 11:12:13"",""format"":""yyyy-MM-dd HH:mm:ss""}}";

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
