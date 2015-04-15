using System;

namespace PactNet.Extensions
{
    public static class StringExtensions
    {
        public static string ToLowerSnakeCase(this string input)
        {
            return !String.IsNullOrEmpty(input) ?
                input.Replace(' ', '_').ToLower() :
                String.Empty;
        }
    }
}