using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Comparers;

namespace PactNet.Matchers
{
    public abstract class MatchCheck
    {
        private const string PathPrefix = "$.";
        private string _path;

        public string Path
        {
            get { return _path; }
            protected set { _path = value.StartsWith(PathPrefix) ? value : PathPrefix + value; }
        }

        public bool Failed { get; protected set; }
        public string Message { get; protected set; }
    }

    public class FailedMatchCheck : MatchCheck
    {
        public FailedMatchCheck(string path, string message)
        {
            Path = path;
            Failed = true;
            Message = message;
        }
    }

    public class SuccessfulMatchCheck : MatchCheck
    {
        public SuccessfulMatchCheck(string path)
        {
            Path = path;
            Failed = false;
        }
    }

    public class MatchResult
    {
        public IEnumerable<MatchCheck> PerformedChecks { get; set; }
    }

    public interface IMatcher
    {
        string MatchPath { get; }
        MatchResult Match(dynamic expected, dynamic actual);
    }

    //TODO: Maybe allowExtraKeys could remove the need for two seperate default body matchers? https://github.com/bethesque/pact-specification/blob/version-2/example.json

    public class DefaultRequestBodyMatcher : IMatcher
    {
        public string MatchPath { get { return "$..*"; } }

        public MatchResult Match(dynamic expected, dynamic actual)
        {
            var checks = new List<MatchCheck>();

            //TODO: Maybe look at changing these to JToken.FromObject(...)
            string expectedJson = JsonConvert.SerializeObject(expected);
            string actualJson = JsonConvert.SerializeObject(actual);
            var expectedToken = JsonConvert.DeserializeObject<JToken>(expectedJson);
            var actualToken = JsonConvert.DeserializeObject<JToken>(actualJson);

            //TODO: This will need to become a full object graph walk / strict compare
            if (!JToken.DeepEquals(expectedToken, actualToken))
            {
                checks.Add(new FailedMatchCheck(MatchPath, new DiffComparisonFailure(expectedToken, actualToken).Result));
            }
            return new MatchResult { PerformedChecks = checks };
        }
    }

    public class DefaultResponseBodyMatcher : IMatcher
    {
        public string MatchPath { get { return "$..*"; } }

        public MatchResult Match(dynamic expected, dynamic actual)
        {
            var checks = new List<MatchCheck>();

            //TODO: Maybe look at changing these to JToken.FromObject(...)
            string expectedJson = JsonConvert.SerializeObject(expected);
            string actualJson = JsonConvert.SerializeObject(actual);
            var expectedToken = JsonConvert.DeserializeObject<JToken>(expectedJson);
            var actualToken = JsonConvert.DeserializeObject<JToken>(actualJson);

            AssertPropertyValuesMatch(expectedToken, actualToken, checks);

            return new MatchResult { PerformedChecks = checks };
        }

        private bool AssertPropertyValuesMatch(JToken httpBody1, JToken httpBody2, List<MatchCheck> checks)
        {
            switch (httpBody1.Type)
            {
                case JTokenType.Array:
                    {
                        if (httpBody1.Count() != httpBody2.Count())
                        {
                            checks.Add(new FailedMatchCheck(httpBody1.Path, new DiffComparisonFailure(httpBody1.Root, httpBody2.Root).Result));
                            return false;
                        }

                        for (var i = 0; i < httpBody1.Count(); i++)
                        {
                            if (httpBody2.Count() > i)
                            {
                                var isMatch = AssertPropertyValuesMatch(httpBody1[i], httpBody2[i], checks);
                                if (!isMatch)
                                {
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case JTokenType.Object:
                    {
                        foreach (JProperty item1 in httpBody1)
                        {
                            var item2 = httpBody2.Cast<JProperty>().SingleOrDefault(x => x.Name == item1.Name);

                            if (item2 != null)
                            {
                                var isMatch = AssertPropertyValuesMatch(item1, item2, checks);
                                if (!isMatch)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                checks.Add(new FailedMatchCheck(httpBody1.Path, new DiffComparisonFailure(httpBody1.Root, httpBody2.Root).Result));
                                return false;
                            }
                        }
                        break;
                    }
                case JTokenType.Property:
                    {
                        var httpBody2Item = httpBody2.SingleOrDefault();
                        var httpBody1Item = httpBody1.SingleOrDefault();

                        if (httpBody2Item == null && httpBody1Item == null)
                        {
                            return true;
                        }

                        if (httpBody2Item != null && httpBody1Item != null)
                        {
                            AssertPropertyValuesMatch(httpBody1Item, httpBody2Item, checks);
                        }
                        else
                        {
                            checks.Add(new FailedMatchCheck(httpBody1.Path, new DiffComparisonFailure(httpBody1.Root, httpBody2.Root).Result));
                            return false;
                        }
                        break;
                    }
                case JTokenType.Integer:
                case JTokenType.String:
                    {
                        if (!httpBody1.Equals(httpBody2))
                        {
                            checks.Add(new FailedMatchCheck(httpBody1.Path, new DiffComparisonFailure(httpBody1.Root, httpBody2.Root).Result));
                            return false;
                        }
                        break;
                    }
                default:
                    {
                        if (!JToken.DeepEquals(httpBody1, httpBody2))
                        {
                            checks.Add(new FailedMatchCheck(httpBody1.Path, new DiffComparisonFailure(httpBody1.Root, httpBody2.Root).Result));
                            return false;
                        }
                        break;
                    }
            }

            return true;
        }
    }

    public class RegExMatcher : IMatcher
    {
        public string MatchPath { get; set; }
        public string RegEx { get; private set; }

        public RegExMatcher(string regex)
        {
            RegEx = regex;
        }

        public MatchResult Match(dynamic expected, dynamic actual)
        {
            //var act = actual as JValue;
            //return act != null && Regex.IsMatch(act.Value.ToString(), RegEx);
            return new MatchResult();
        }
    }

    public abstract class Matcher
    {
        protected Matcher(dynamic example)
        {
            Example = example;
        }

        public string Path { get; set; }
        public dynamic Example { get; set; }

        [JsonProperty("$type")]
        public string Name
        {
            get { return GetType().FullName; }
        }

        public abstract dynamic ResponseMatchingRule { get; }

        /*public static TypeMatcherDefinition TypeEg(object example)
        {
            return new TypeMatcherDefinition(example);
        }

        public static TypeMatcherDefinition AllElementsInArrayTypeEg(object example)
        {
            return new TypeMatcherDefinition(new List<dynamic> { example }, "[*]");
        }

        public static TypeMatcherDefinition MatchTypeToAllPropertiesInObjectEg(object example)
        {
            return new TypeMatcherDefinition(example, ".*");
        }*/

        public static IMatcher RegExEg(object example, string regEx)
        {
            //return new RegExMatcher(example, regEx);
            return null;
        }

        /*public static MinMatcherDefinition MinEg(IEnumerable<dynamic> example, int minimum)
        {
            return new MinMatcherDefinition(example, minimum);
        }*/
    }
}