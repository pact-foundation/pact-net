using System;
using System.Threading.Tasks;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Message scenarios
    /// </summary>
    public class MessageScenarios : IMessageScenarios
    {
        private static readonly dynamic JsonMetadata = new
        {
            ContentType = "application/json"
        };

        /// <summary>
        /// Add a message scenario
        /// </summary>
        /// <param name="description">Scenario description</param>
        /// <param name="factory">Message content factory</param>
        public IMessageScenarios Add(string description, Func<dynamic> factory)
        {
            var scenario = new Scenario(description, factory, JsonMetadata, null);
            Scenarios.AddScenario(scenario);

            return this;
        }

        /// <summary>
        /// Add a message scenario
        /// </summary>
        /// <param name="description">Scenario description</param>
        /// <param name="configure">Scenario configure</param>
        /// <returns></returns>
        public IMessageScenarios Add(string description, Action<IMessageScenarioBuilder> configure)
        {
            var builder = new MessageScenarioBuilder(description);
            configure?.Invoke(builder);

            Scenario scenario = builder.Build();
            Scenarios.AddScenario(scenario);

            return this;
        }

        /// <summary>
        /// Add a message scenario
        /// </summary>
        /// <param name="description">Scenario description</param>
        /// <param name="configure">Scenario configure</param>
        /// <returns></returns>
        public IMessageScenarios Add(string description, Func<IMessageScenarioBuilder, Task> configure)
        {
            var builder = new MessageScenarioBuilder(description);
            configure?.Invoke(builder).GetAwaiter().GetResult();

            Scenario scenario = builder.Build();
            Scenarios.AddScenario(scenario);

            return this;
        }
    }
}
