using System;
using System.Collections.Generic;

namespace PactNet
{
    public class PactVerifierConfig
    {
        public string LogDir { get; set; }

        public IEnumerable<Action<string>> Reporters { get; set; }

        internal string LoggerName;

        public PactVerifierConfig()
        {
            LogDir = Constants.DefaultLogDir;
            Reporters = new List<Action<string>>();
        }
    }
}