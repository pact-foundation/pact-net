using System.Collections.Generic;
using PactNet.Infrastructure.Outputters;

namespace PactNet
{
    /// <summary>
    /// Pact verifier configuration
    /// </summary>
    public class PactVerifierConfig
    {
        /// <summary>
        /// Log outputs
        /// </summary>
        public IEnumerable<IOutput> Outputters { get; set; } = new List<IOutput>
        {
            new ConsoleOutput()
        };

        /// <summary>
        /// Publish verification results?
        /// </summary>
        public bool PublishVerificationResults { get; set; }

        /// <summary>
        /// Provider version
        /// </summary>
        /// <remarks>Required if publishing verification results</remarks>
        public string ProviderVersion { get; set; }

        /// <summary>
        /// Provider tags applied when publishing verification results
        /// </summary>
        public ICollection<string> ProviderTags { get; set; } = new List<string>();
    }
}