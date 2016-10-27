using System;
using System.Linq;
using System.Linq.Expressions;

namespace PactNet.Extensions
{
    public static class StringExtensions
    {
        public static string ToLowerSnakeCase(this string input)
        {
            return !string.IsNullOrEmpty(input)
                ? input.Replace(' ', '_').ToLower()
                : string.Empty;
        }

        public static Func<string> ToFunc(this string expressionStr)
        {
            try {
                var split = expressionStr?.Split(new[] { "=>" }, StringSplitOptions.None);

                if (split?.Length != 2)
                    return null;

                var value = split.LastOrDefault()?.TrimStart().TrimEnd();

                if (string.IsNullOrEmpty(value))
                    return null;

                var expression = Expression.Convert(Expression.Constant(value), typeof(string));

                return Expression
                    .Lambda<Func<string>>(expression)
                    .Compile();
            }
            catch {
                return null;
            }
        }
    }
}