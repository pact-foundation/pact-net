using Newtonsoft.Json;

namespace PactNet.Matchers.Type
{
    public class TypeMatcher : IMatcher
    {
        public string Type => "type";

        public dynamic Value { get; }

        public TypeMatcher(dynamic example)
        {
            this.Value = example;
        }
    }
}