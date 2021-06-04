using System;

namespace PactNet
{
    /// <summary>
    /// Context for a running mock server
    /// </summary>
    /// <remarks>When disposed, the configured interactions are verified and the pact file is written if they all succeed</remarks>
    public interface IPactContext : IDisposable
    {
        /// <summary>
        /// URI for the mock server
        /// </summary>
        Uri MockServerUri { get; }
    }
}