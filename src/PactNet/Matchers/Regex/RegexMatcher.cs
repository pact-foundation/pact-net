using Newtonsoft.Json;

namespace PactNet.Matchers.Regex
{
    public class RegexMatcher : IMatcher
    {
        //Generate JSON using the Ruby spec for now

        [JsonProperty(PropertyName = "json_class")]
        public string Match { get; set; }

        [JsonProperty(PropertyName = "data")]
        public dynamic Example { get; set; }

        internal RegexMatcher(string example, string regex)
        {
            Match = "Pact::Term";
            Example = new
            {
                generate = example,
                matcher = new
                {
                    json_class = "Regexp",
                    o = 0,
                    s = regex
                }
            };
        }
    }
}