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
        /// Pact config
        /// </summary>
        PactConfig Config { get; }
    }

    /// <summary>
    /// Marker interface for a v2 Pact
    /// </summary>
    public interface IPactV2 : IPact
    {
    }

    /// <summary>
    /// Marker interface for a v3 Pact
    /// </summary>
    public interface IPactV3 : IPact
    {
    }
}
