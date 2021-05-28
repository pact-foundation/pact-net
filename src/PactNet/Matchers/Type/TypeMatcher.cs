using Newtonsoft.Json;

namespace PactNet.Matchers.Type
{
    public class TypeMatcher : IMatcher
    {
        //Generate JSON using the Ruby spec for now

        [JsonProperty(PropertyName = "json_class")]
        public string Match { get; set; }

        [JsonProperty(PropertyName = "contents")]
        public dynamic Example { get; set; }

        public TypeMatcher(dynamic example)
        {
            Match = "Pact::SomethingLike";
            Example = example;
        }
    }
}