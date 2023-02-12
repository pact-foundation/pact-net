namespace PactNet.Verifier
{
    /// <summary>
    /// Style for passing provider state information
    /// </summary>
    public enum ProviderStateStyle
    {
        /// <summary>
        /// Pass provider state information in the request body (default)
        /// </summary>
        Body,

        /// <summary>
        /// Pass provider state information as query string parameters
        /// </summary>
        Query
    }
}
