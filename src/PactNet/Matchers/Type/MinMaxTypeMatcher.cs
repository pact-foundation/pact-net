using System;
using Newtonsoft.Json;

namespace PactNet.Matchers.Type
{
    /// <summary>
    /// Match every element of a collection with a min and/or max size against an example matcher
    /// </summary>
    public class MinMaxTypeMatcher : IMatcher
    {
        public string Type => "type";

        public dynamic Value { get; }

        [JsonProperty(PropertyName = "min", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Min { get; set; }

        [JsonProperty(PropertyName = "max", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Max { get; set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="MinMaxTypeMatcher"/> class.
        /// </summary>
        /// <param name="example">Example matcher</param>
        /// <param name="min">Minimum collection size</param>
        /// <param name="max">Maximum collection size</param>
        public MinMaxTypeMatcher(dynamic example, int min = default, int max = default)
        {
            if (min == default && max == default)
            {
                throw new ArgumentException("You must specify at least one of min and max. Min must be > 0");
            }

            this.Value = example;
            this.Min = min;
            this.Max = max;
        }
    }
}
