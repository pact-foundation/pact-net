using System;
using System.Text.Json;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Defines the scenario model
    /// </summary>
    public class Scenario
    {
        private readonly Func<dynamic> factory;

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
        public Scenario(string description, Func<dynamic> factory)
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
            : this(description, factory)
        {
            this.Metadata = metadata;
            this.JsonSettings = settings;
        }

        /// <summary>
        /// Invoke a scenario
        /// </summary>
        /// <returns>The scenario message content</returns>
        public dynamic Invoke()
        {
            return this.factory.Invoke();
        }
    }
}
