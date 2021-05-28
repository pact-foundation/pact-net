using Newtonsoft.Json;

namespace PactNet.Matchers.Regex
{
    public class RegexMatcher : IMatcher
    {
        public string Type => "regex";

        public dynamic Value { get; }

        [JsonProperty("regex")]
        public string Regex { get; }

        internal RegexMatcher(string example, string regex)
        {
            this.Regex = regex;
            this.Value = example;
        }
    }
}