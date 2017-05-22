using PactNet.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpQueryStringComparer : IHttpQueryStringComparer
    {
        public ComparisonResult Compare(string expected, string actual)
        {
            if (string.IsNullOrEmpty(expected) && string.IsNullOrEmpty(actual))
            {
                return new ComparisonResult("has no query strings");
            }

            string normalisedExpectedQuery = NormaliseUrlEncodingAndTrimTrailingAmpersand(expected);
            string normalisedActualQuery = NormaliseUrlEncodingAndTrimTrailingAmpersand(actual);
            var result = new ComparisonResult("has query {0}", normalisedExpectedQuery ?? "null");

            if (expected == null || actual == null)
            {
                result.RecordFailure(new DiffComparisonFailure(expected, actual));
            }
            else
            {
                IDictionary<string, IList<string>> expectedQueryItems = ParseQueryString(normalisedExpectedQuery);
                IDictionary<string, IList<string>> actualQueryItems = ParseQueryString(normalisedActualQuery);

                if (actualQueryItems.Keys.Count != expectedQueryItems.Keys.Count || actualQueryItems.Keys.Except(expectedQueryItems.Keys).Any())
                {
                    result.RecordFailure(new DiffComparisonFailure(normalisedExpectedQuery, normalisedActualQuery));
                }
                else if (expectedQueryItems.Keys.Any(expectedKey => !actualQueryItems[expectedKey].SequenceEqual(expectedQueryItems[expectedKey])))
                {
                    result.RecordFailure(new DiffComparisonFailure(normalisedExpectedQuery, normalisedActualQuery));
                }
            }

            return result;
        }

        private void AddOrAppendResult(IDictionary<string, IList<string>> results, string key, string value)
        {
            if (results.TryGetValue(key, out IList<string> values))
            {
                values.Add(value);
            }
            else
            {
                results[key] = new List<string> { value };
            }
        }

        private string NormaliseUrlEncodingAndTrimTrailingAmpersand(string query)
        {
            return query == null ? null : Regex.Replace(query, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper()).TrimEnd('&');
        }

        private IDictionary<string, IList<string>> ParseQueryString(string queryString)
        {
            var results = new Dictionary<string, IList<string>>();

            if (string.IsNullOrEmpty(queryString) || queryString == "?")
            {
                return null;
            }

            int scanIndex = 0;

            if (queryString[0] == '?')
            {
                scanIndex = 1;
            }

            int textLength = queryString.Length;
            int equalIndex = queryString.IndexOf('=');

            if (equalIndex == -1)
            {
                equalIndex = textLength;
            }

            while (scanIndex < textLength)
            {
                int delimiterIndex = queryString.IndexOf('&', scanIndex);

                if (delimiterIndex == -1)
                {
                    delimiterIndex = textLength;
                }

                if (equalIndex < delimiterIndex)
                {
                    while (scanIndex != equalIndex && char.IsWhiteSpace(queryString[scanIndex]))
                    {
                        ++scanIndex;
                    }

                    string name = queryString.Substring(scanIndex, equalIndex - scanIndex);
                    string value = queryString.Substring(equalIndex + 1, delimiterIndex - equalIndex - 1);

                    AddOrAppendResult(results, Uri.UnescapeDataString(name.Replace('+', ' ')), Uri.UnescapeDataString(value.Replace('+', ' ')));

                    equalIndex = queryString.IndexOf('=', delimiterIndex);

                    if (equalIndex == -1)
                    {
                        equalIndex = textLength;
                    }
                }
                else
                {
                    if (delimiterIndex > scanIndex)
                    {
                        AddOrAppendResult(results, queryString.Substring(scanIndex, delimiterIndex - scanIndex), string.Empty);
                    }
                }

                scanIndex = delimiterIndex + 1;
            }

            return results;
        }
    }
}