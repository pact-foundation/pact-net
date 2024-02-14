using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Defines the message scenario builder
    /// </summary>
    internal class MessageScenarioBuilder : IMessageScenarioBuilder
    {
        private readonly string description;
        private Func<dynamic> factory;
        private dynamic metadata = new { ContentType = "application/json" };
        private JsonSerializerOptions settings;

        /// <summary>
        /// Initialises a new instance of the <see cref="MessageScenarioBuilder"/> class.
        /// </summary>
        /// <param name="description">Scenario description</param>
        public MessageScenarioBuilder(string description)
        {
            this.description = !string.IsNullOrWhiteSpace(description) ? description : throw new ArgumentNullException(nameof(description));
        }

        /// <summary>
        /// Set the metadata of the message content
        /// </summary>
        /// <param name="metadata">the metadata</param>
        /// <returns>Fluent builder</returns>
        public IMessageScenarioBuilder WithMetadata(dynamic metadata)
        {
            this.metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            return this;
        }

        /// <summary>
        /// Set the action of the scenario
        /// </summary>
        /// <param name="factory">Content factory</param>
        public void WithContent(Func<dynamic> factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Set the content of the scenario
        /// </summary>
        /// <param name="factory">Content factory</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        public void WithContent(Func<dynamic> factory, JsonSerializerOptions settings)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Set the action of the scenario
        /// </summary>
        /// <param name="factory">Content factory</param>
        public async Task WithContentAsync(Func<Task<dynamic>> factory)
        {
            dynamic value = await factory();
            this.factory = () => value;
        }

        /// <summary>
        /// Set the content of the scenario
        /// </summary>
        /// <param name="factory">Content factory</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        public async Task WithContentAsync(Func<Task<dynamic>> factory, JsonSerializerOptions settings)
        {
            dynamic value = await factory();
            this.factory = () => value;
            this.settings = settings;
        }

        /// <summary>
        /// Build the scenario
        /// </summary>
        internal Scenario Build()
        {
            if (this.factory == null)
            {
                throw new InvalidOperationException("You must set the content of a scenario");
            }

            return new Scenario(this.description, this.factory, this.metadata, this.settings);
        }
    }
}
