using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using PactNet.Extensions;
using PactNet.Matchers;

namespace PactNet.Mocks.MockHttpService.Matchers
{
    internal class DefaultHttpBodyMatcher : IMatcher
    {
        public const string Path = "$..*";

        public bool AllowExtraKeys { get; private set; }

        public DefaultHttpBodyMatcher(bool allowExtraKeysInObjects)
        {
            AllowExtraKeys = allowExtraKeysInObjects;
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            if (expected is JValue)
            {
                return actual != null && expected.Equals(actual) ?
                    new MatcherResult(new SuccessfulMatcherCheck(path)) :
                    new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueDoesNotMatch));
            }

            var checks = new List<MatcherCheck>();

            var expectedTokens = expected.SelectTokens(path);

            foreach (var expectedToken in expectedTokens)
            {
                if (expectedToken is JArray || (!AllowExtraKeys && expectedToken is JObject))
                {
                    var actualToken = actual.SelectToken(expectedToken.Path);

                    if (actualToken != null && expectedToken.Count().Equals(actualToken.Count()))
                    {
                        checks.Add(new SuccessfulMatcherCheck(expectedToken.Path));
                    }
                    else
                    {
                        var failureType = expectedToken is JArray ? 
                            MatcherCheckFailureType.AdditionalItemInArray : 
                            MatcherCheckFailureType.AdditionalPropertyInObject;
                        
                        checks.Add(new FailedMatcherCheck(expectedToken.Path, failureType));
                    }
                }
                else if (expectedToken is JValue) {
                    var actualToken = actual.SelectToken(expectedToken.Path);

                    var func = expectedToken.Value<string>()?.ToFunc();

                    if (func != null && Regex.IsMatch(actualToken.Value<string>(), func.Invoke()))
                        checks.Add(new SuccessfulMatcherCheck(expectedToken.Path));

                    else if (expectedToken.Equals(actualToken))
                        checks.Add(new SuccessfulMatcherCheck(expectedToken.Path));

                    else
                        checks.Add(new FailedMatcherCheck(expectedToken.Path, MatcherCheckFailureType.ValueDoesNotMatch));
                }
            }

            return new MatcherResult(checks);
        }
    }
}