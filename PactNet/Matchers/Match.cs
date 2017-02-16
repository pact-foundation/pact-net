using System;
using PactNet.Matchers.Date;
using PactNet.Matchers.Decimal;
using PactNet.Matchers.Equality;
using PactNet.Matchers.Integer;
using PactNet.Matchers.Max;
using PactNet.Matchers.Min;
using PactNet.Matchers.Regex;
using PactNet.Matchers.Timestamp;
using PactNet.Matchers.Type;

namespace PactNet.Matchers
{
    public static class Match
    {
        public static MatchDefinition Regex(dynamic example, string regex)
        {
            return new RegexMatchDefinition(example, regex);
        }

        public static MatchDefinition Type(dynamic example)
        {
            return new TypeMatchDefinition(example);
        }

        public static MatchDefinition Equality(dynamic example)
        {
            return new EqualityMatchDefinition(example);
        }

        public static MatchDefinition Integer(dynamic example)
        {
            return new IntegerMatchDefinition(example);
        }

        public static MatchDefinition Decimal(dynamic example)
        {
            return new DecimalMatchDefinition(example);
        }

        public static MatchDefinition Min(dynamic example, int minValue)
        {
            return new MinMatchDefinition(example, minValue);
        }

        public static MatchDefinition Max(dynamic example, int maxValue)
        {
            return new MaxMatchDefinition(example, maxValue);
        }

        public static MatchDefinition Date(dynamic example, string format)
        {
            return new DateFormatMatchDefinition(example, format);
        }

        public static MatchDefinition Timestamp(dynamic example, string format)
        {
            return new TimestampDefinition(example, format);
        }

        public static MatchDefinition Include(dynamic example)
        {
            throw new NotImplementedException();
        }
    }
}