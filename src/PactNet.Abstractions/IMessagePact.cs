using System;

namespace PactNet
{
    /// <summary>
    /// A messaging messagePact between a consumer and a provider
    /// </summary>
    [Obsolete("Use IPact instead")]
    public interface IMessagePact
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
    /// Marker interface for a v3 message Pact
    /// </summary>
    [Obsolete("Use IPactV3 instead")]
    public interface IMessagePactV3 : IMessagePact
    {
    }
}
