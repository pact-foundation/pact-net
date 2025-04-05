using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Defines the scenario model
    /// </summary>
    public class Scenario
    {
        private readonly Func<Task<dynamic>> factory;

        /// <summary>
        /// The description of the scenario
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Message metadata
        /// </summary>
        public dynamic Metadata { get; }

        /// <summary>
        /// Custom JSON serializer settings
        /// </summary>
        public JsonSerializerOptions JsonSettings { get; }

        /// <summary>
        /// Creates an instance of <see cref="Scenario"/>
        /// </summary>
        /// <param name="description">the scenario description</param>
        /// <param name="factory">Message content factory</param>
        public Scenario(string description, Func<dynamic> factory) : this(description, Wrap(factory))
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="Scenario"/>
        /// </summary>
        /// <param name="description">the scenario description</param>
        /// <param name="factory">Message content factory</param>
        public Scenario(string description, Func<Task<dynamic>> factory)
        {
            this.Description = !string.IsNullOrWhiteSpace(description) ? description : throw new ArgumentException("Description cannot be null or empty");
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Creates an instance of <see cref="Scenario"/>
        /// </summary>
        /// <param name="description">the scenario description</param>
        /// <param name="factory">Message content factory</param>
        /// <param name="metadata">the metadata</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        public Scenario(string description, Func<dynamic> factory, dynamic metadata, JsonSerializerOptions settings)
            : this(description, Wrap(factory))
        {
            this.Metadata = metadata;
            this.JsonSettings = settings;
        }

        /// <summary>
        /// Creates an instance of <see cref="Scenario"/>
        /// </summary>
        /// <param name="description">the scenario description</param>
        /// <param name="factory">Message content factory</param>
        /// <param name="metadata">the metadata</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        public Scenario(string description, Func<Task<dynamic>> factory, dynamic metadata, JsonSerializerOptions settings)
            : this(description, factory)
        {
            this.Metadata = metadata;
            this.JsonSettings = settings;
        }

        /// <summary>
        /// Invoke a scenario to generate message content
        /// </summary>
        /// <returns>The scenario message content</returns>
        public async Task<dynamic> InvokeAsync() => await this.factory();

        /// <summary>
        /// Wraps a sync factory to be async
        /// </summary>
        /// <param name="factory">Sync factory</param>
        /// <returns>Async factory</returns>
        private static Func<Task<dynamic>> Wrap(Func<dynamic> factory) => () =>
        {
            dynamic d = factory();
            return Task.FromResult(d);
        };
    }
}
