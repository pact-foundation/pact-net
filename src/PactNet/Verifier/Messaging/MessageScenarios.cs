using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// The scenarios used to generate messages when verifying messaging pacts
    /// </summary>
    internal class MessageScenarios : IMessageScenarios
    {
        private static readonly dynamic JsonMetadata = new
        {
            ContentType = "application/json"
        };

        private readonly IDictionary<string, Scenario> scenarios;

        /// <summary>
        /// Configured scenarios
        /// </summary>
        public IReadOnlyDictionary<string, Scenario> Scenarios => new ReadOnlyDictionary<string, Scenario>(this.scenarios);

        /// <summary>
        /// Initialises a new instance of the <see cref="MessageScenarios"/> class.
        /// </summary>
        public MessageScenarios()
        {
            this.scenarios = new Dictionary<string, Scenario>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Add a message scenario
        /// </summary>
        /// <param name="description">Scenario description</param>
        /// <param name="factory">Message content factory</param>
        public IMessageScenarios Add(string description, Func<dynamic> factory)
        {
            var scenario = new Scenario(description, factory, JsonMetadata, null);
            this.scenarios.Add(description, scenario);

            return this;
        }

        /// <summary>
        /// Add a message scenario by configuring a scenario builder
        /// </summary>
        /// <param name="description">Scenario description</param>
        /// <param name="configure">Scenario configure</param>
        /// <returns>Fluent builder</returns>
        public IMessageScenarios Add(string description, Action<IMessageScenarioBuilder> configure)
        {
            var builder = new MessageScenarioBuilder(description);
            configure?.Invoke(builder);

            Scenario scenario = builder.Build();
            this.scenarios.Add(description, scenario);

            return this;
        }
    }
}
