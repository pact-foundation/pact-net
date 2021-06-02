using System;
using Newtonsoft.Json;

namespace PactNet.Matchers.Type
{
    public class MinTypeMatcher : IMatcher
    {
        public string Type => "type";

        public dynamic Value { get; }

        [JsonProperty(PropertyName = "min")]
        public int Min { get; set; }

        public MinTypeMatcher(dynamic example, int min)
        {
            if (min < 1)
            {
                throw new ArgumentException("Min must be greater than 0");
            }

            this.Value = new[] { example };
            this.Min = min;
        }
    }
}
