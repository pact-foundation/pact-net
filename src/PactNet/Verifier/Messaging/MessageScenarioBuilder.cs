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

        private Func<Task<dynamic>> factory;
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
        /// Set the content factory of the scenario. The factory is invoked each time the scenario is required.
        /// </summary>
        /// <param name="factory">Content factory</param>
        public void WithContent(Func<dynamic> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.WithAsyncContent(() => Task.FromResult<dynamic>(factory()));
        }

        /// <summary>
        /// Set the content factory of the scenario. The factory is invoked each time the scenario is required.
        /// </summary>
        /// <param name="factory">Content factory</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        public void WithContent(Func<dynamic> factory, JsonSerializerOptions settings)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.WithAsyncContent(() => Task.FromResult<dynamic>(factory()), settings);
        }

        /// <summary>
        /// Set the content factory of the scenario. The factory is invoked each time the scenario is required.
        /// </summary>
        /// <param name="factory">Content factory</param>
        public void WithAsyncContent(Func<Task<dynamic>> factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Set the content factory of the scenario. The factory is invoked each time the scenario is required.
        /// </summary>
        /// <param name="factory">Content factory</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        public void WithAsyncContent(Func<Task<dynamic>> factory, JsonSerializerOptions settings)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
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
