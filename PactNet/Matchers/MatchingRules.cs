using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PactNet.Matchers
{
    public class MatchingRules
    {
        public MatchingRules()
        {
            this.Matchers = new List<IMatcher>();
        }

        [JsonProperty("matchers")]
        public List<IMatcher> Matchers { get; set; }

        public void Add(IMatcher matcher)
        {
            this.Matchers.Add(matcher);
        }

        public void Add(IEnumerable<IMatcher> matchers)
        {
            foreach (var matcher in matchers)
                this.Add(matcher);
        }

        public void Add(MatchingRules rules)
        {
            foreach (var matcher in rules.Matchers)
                this.Add(matcher);
        }
    }
}
