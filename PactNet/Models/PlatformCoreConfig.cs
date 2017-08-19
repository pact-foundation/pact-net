using System.Collections.Generic;

namespace PactNet.Models
{
    internal class PlatformCoreConfig
    {
        public string FileName { get; set; }
        public string Arguments { get; set; }
        public IDictionary<string, string> Environment { get; set; }
    }
}
