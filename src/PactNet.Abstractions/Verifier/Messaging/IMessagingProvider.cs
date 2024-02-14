using System;
using System.Text.Json;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Messaging provider service, which simulates messaging responses in order to verify interactions
    /// </summary>
    public interface IMessagingProvider : IDisposable
    {
        /// <summary>
        /// Scenarios configured for the provider
        /// </summary>
        IMessageScenarios Scenarios { get; }

        /// <summary>
        /// Start the provider service
        /// </summary>
        /// <param name="settings">Default JSON serializer settings</param>
        /// <returns>URI of the started service</returns>
        Uri Start(JsonSerializerOptions settings);
    }
}
