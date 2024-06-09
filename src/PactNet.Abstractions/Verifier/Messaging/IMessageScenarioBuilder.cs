using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Defines the message scenario builder interface
    /// </summary>
    public interface IMessageScenarioBuilder
    {
        /// <summary>
        /// Set the metadata of the message content
        /// </summary>
        /// <param name="metadata">the metadata</param>
        /// <returns>Fluent builder</returns>
        IMessageScenarioBuilder WithMetadata(dynamic metadata);

        /// <summary>
        /// Set the action of the scenario
        /// </summary>
        /// <param name="factory">Content factory</param>
        void WithContent(Func<dynamic> factory);

        /// <summary>
        /// Set the content of the scenario
        /// </summary>
        /// <param name="factory">Content factory</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        void WithContent(Func<dynamic> factory, JsonSerializerOptions settings);

        /// <summary>
        /// Set the action of the scenario
        /// </summary>
        /// <param name="factory">Content factory</param>
        Task WithContentAsync(Func<Task<dynamic>> factory);

        /// <summary>
        /// Set the content of the scenario
        /// </summary>
        /// <param name="factory">Content factory</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        Task WithContentAsync(Func<Task<dynamic>> factory, JsonSerializerOptions settings);
    }
}
