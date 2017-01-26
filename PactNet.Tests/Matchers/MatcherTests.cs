using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute.Core;
using PactNet.Matchers;
using PactNet.Matchers.Date;
using PactNet.Matchers.Decimal;
using PactNet.Matchers.Equality;
using PactNet.Matchers.Integer;
using PactNet.Matchers.Max;
using PactNet.Matchers.Min;
using PactNet.Matchers.Timestamp;
using PactNet.Matchers.Type;
using PactNet.Mocks.MockHttpService.Matchers.Regex;
using Xunit;

namespace PactNet.Tests.Matchers
{
    public class MatcherTests
    {
        [Fact]
        public void TypeMatcher_Succeeds_When_Same_Type()
        {
            var matcher = new TypeMatcher();

            var actual = new JValue(4);
            var expected = new JValue(9);

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));
        }

        [Fact]
        public void TypeMatcher_Fails_When_Different_Type()
        {
            var matcher = new TypeMatcher();

            var actual = new JValue("4");
            var expected = new JValue(9);

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));

            Assert.Equal(MatcherCheckFailureType.ValueDoesNotMatch,
                ((FailedMatcherCheck) result.MatcherChecks.First(m => m.GetType() == typeof(FailedMatcherCheck)))
                .FailureType);
            
        }

        [Fact]
        public void EqualityMatcher_Succeeds_When_Equal()
        {
            var matcher = new EqualityMatcher();

            var actual = new JValue("test");
            var expected = new JValue("test");

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));
        }

        [Fact]
        public void EqualityMatcher_Fails_When_Not_Equal()
        {
            var matcher = new EqualityMatcher();

            var actual = new JValue("test");
            var expected = new JValue("nottest");

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));

            Assert.Equal(MatcherCheckFailureType.ValueNotEqual,
                ((FailedMatcherCheck)result.MatcherChecks.First(m => m.GetType() == typeof(FailedMatcherCheck)))
                .FailureType);

        }

        [Fact]
        public void IntegerMatcher_Succeeds_When_Integer()
        {
            var matcher = new IntegerMatcher();

            var actual = new JValue(8);
            var expected = new JValue(8);

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));
        }

        [Fact]
        public void IntegerMatcher_Fails_When_Not_Integer()
        {
            var matcher = new IntegerMatcher();

            var actual = new JValue(1.5);
            var expected = new JValue(1);

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));

            Assert.Equal(MatcherCheckFailureType.ValueNotInteger,
                ((FailedMatcherCheck)result.MatcherChecks.First(m => m.GetType() == typeof(FailedMatcherCheck)))
                .FailureType);

        }

        [Fact]
        public void DecimalMatcher_Succeeds_When_Decimal()
        {
            var matcher = new DecimalMatcher();

            var actual = new JValue(1.5); //has to be a decimal type (not string type)
            var expected = new JValue(1.8);

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));
        }

        [Fact]
        public void DecimalMatcher_Fails_When_Not_Decimal()
        {
            var matcher = new DecimalMatcher();

            var actual = new JValue("1.5");
            var expected = new JValue(1.8);

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));

            Assert.Equal(MatcherCheckFailureType.ValueNotDecimal,
                ((FailedMatcherCheck)result.MatcherChecks.First(m => m.GetType() == typeof(FailedMatcherCheck)))
                .FailureType);

        }

        [Fact]
        public void DateFormatMatcher_Succeeds_When_Following_DateFormat()
        {
            var matcher = new DateFormatMatcher("MM/dd/yyyy");

            var actual = new JValue("01/01/2017");
            var expected = new JValue("01/01/2017");

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));
        }

        [Fact]
        public void DateFormatMatcher_Fails_When_Not_Following_DateFormat()
        {
            var matcher = new DateFormatMatcher("MM/dd/yyyy");

            var actual = new JValue("2017/01/01");
            var expected = new JValue("01/01/2017");

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));

            Assert.Equal(MatcherCheckFailureType.ValueDoesNotMatchDateFormat,
                ((FailedMatcherCheck)result.MatcherChecks.First(m => m.GetType() == typeof(FailedMatcherCheck)))
                .FailureType);

        }

        [Fact]
        public void TimestampMatcher_Succeeds_When_Following_Format()
        {
            var matcher = new TimestampMatcher("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");

            var actual = new JValue("2017-01-24T10:08:12.SSSZ");
            var expected = new JValue("2017-01-24T10:08:12.SSSZ");

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));
        }

        [Fact]
        public void TimestampMatcher_Fails_When_Not_Following_Format()
        {
            var matcher = new TimestampMatcher("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");

            var actual = new JValue("2017-01-24T10:08:12");
            var expected = new JValue("2017-01-24T10:08:12.SSSZ");

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));

            Assert.Equal(MatcherCheckFailureType.ValueDoesNotMatchTimestamp,
                ((FailedMatcherCheck)result.MatcherChecks.First(m => m.GetType() == typeof(FailedMatcherCheck)))
                .FailureType);

        }

        [Fact]
        public void RegexMatcher_Succeeds_When_Matches_Regex()
        {
            var matcher = new RegexMatcher("([a-z]).*");

            var actual = new JValue("lowercase");
            var expected = new JValue("lowercase");

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));
        }

        [Fact]
        public void RegexMatcher_Fails_When_Not_Matching_Regex()
        {
            var matcher = new RegexMatcher("([a-z]).*");

            var actual = new JValue("UPPERCASE");
            var expected = new JValue("lowercase");

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));

            Assert.Equal(MatcherCheckFailureType.ValueDoesNotMatch,
                ((FailedMatcherCheck)result.MatcherChecks.First(m => m.GetType() == typeof(FailedMatcherCheck)))
                .FailureType);

        }

        [Fact]
        public void MinMatcher_Succeeds_When_Containing_More_Than_Min_Items()
        {
            var matcher = new MinMatcher(2);

            var actual = new JArray();
            actual.Add(new JValue("item1"));
            actual.Add(new JValue("item2"));
            actual.Add(new JValue("item3"));

            var expected = new JArray();
            expected.Add(new JValue("item1"));
            expected.Add(new JValue("item2"));


            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));
        }

        [Fact]
        public void MinMatcher_Fails_When_Containing_Less_Than_Min_Items()
        {
            var matcher = new MinMatcher(2);

            var actual = new JArray();
            actual.Add(new JValue("item1"));

            var expected = new JArray();
            expected.Add(new JValue("item1"));
            expected.Add(new JValue("item2"));

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));

            Assert.Equal(MatcherCheckFailureType.NotEnoughValuesInArray,
                ((FailedMatcherCheck)result.MatcherChecks.First(m => m.GetType() == typeof(FailedMatcherCheck)))
                .FailureType);

        }

        [Fact]
        public void MaxMatcher_Succeeds_When_Containing_Less_Than_Max_Items()
        {
            var matcher = new MaxMatcher(2);

            var actual = new JArray();
            actual.Add(new JValue("item1"));

            var expected = new JArray();
            expected.Add(new JValue("item1"));

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));
        }

        [Fact]
        public void MaxMatcher_Fails_When_Containing_More_Than_Max_Items()
        {
            var matcher = new MaxMatcher(2);

            var actual = new JArray();
            actual.Add(new JValue("item1"));
            actual.Add(new JValue("item2"));
            actual.Add(new JValue("item3"));

            var expected = new JArray();
            expected.Add(new JValue("item1"));
            expected.Add(new JValue("item2"));

            var result = matcher.Match(string.Empty, expected, actual);

            Assert.Equal(0, result.MatcherChecks.Count(m => m.GetType() == typeof(SuccessfulMatcherCheck)));
            Assert.Equal(1, result.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));

            Assert.Equal(MatcherCheckFailureType.AdditionalItemInArray,
                ((FailedMatcherCheck)result.MatcherChecks.First(m => m.GetType() == typeof(FailedMatcherCheck)))
                .FailureType);

        }
    }
}
