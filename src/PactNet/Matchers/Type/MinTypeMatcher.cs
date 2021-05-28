using Newtonsoft.Json;
using System;

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

            this.Value = example;
            this.Min = min;
        }
    }
}