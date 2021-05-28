namespace PactNet
{
    /// <summary>
    /// A pact between a consumer and a provider
    /// </summary>
    public interface IPact
    {
        /// <summary>
        /// Consumer name
        /// </summary>
        string Consumer { get; }

        /// <summary>
        /// Provider name
        /// </summary>
        string Provider { get; }

        /// <summary>
        /// Specification version
        /// </summary>
        // TODO: make this a proper enum
        string SpecificationVersion { get; }

        /// <summary>
        /// Pact config
        /// </summary>
        PactConfig Config { get; }
    }
}