using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        void WithContent(Func<dynamic> factory, JsonSerializerSettings settings);

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
        Task WithContentAsync(Func<Task<dynamic>> factory, JsonSerializerSettings settings);
    }
}
