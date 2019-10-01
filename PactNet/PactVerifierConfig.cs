using System.Collections.Generic;
using PactNet.Infrastructure.Outputters;

namespace PactNet
{
    public class PactVerifierConfig
    {
        public IEnumerable<IOutput> Outputters { get; set; }
        
        public bool PublishVerificationResults { get; set; }

        public string ProviderVersion { get; set; }

        private KeyValuePair<string, string>? _customHeader;
        public KeyValuePair<string, string>? CustomHeader { // backward compatibility
            get => this._customHeader;
            set
            {
                if (value != null)
                {
                    this.CustomHeaders.Add(value.Value.Key, value.Value.Value);
                }
                if (this._customHeader != null)
                {
                    this.CustomHeaders.Remove(this._customHeader.Value.Value);
                }
                this._customHeader = value;
            }
        }
        public Dictionary<string, string> CustomHeaders { get; set; } = new Dictionary<string, string>();

        public bool Verbose { get; set; }

        public string MonkeyPatchFile { get; set; }

        public PactVerifierConfig()
        {
            Outputters = new List<IOutput>
            {
                new ConsoleOutput()
            };
        }
    }
}