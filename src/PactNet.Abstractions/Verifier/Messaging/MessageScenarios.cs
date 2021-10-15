using System;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Message scenarios
    /// </summary>
    public class MessageScenarios : IMessageScenarios
    {
        /// <summary>
        /// Add a configured scenario
        /// </summary>
        /// <param name="scenario">Scenario</param>
        public IMessageScenarios Add(Scenario scenario)
        {
            Scenarios.AddScenario(scenario);
            return this;
        }

        /// <summary>
        /// Add a message scenario
        /// </summary>
        /// <param name="description">Scenario description</param>
        /// <param name="factory">Message content factory</param>
        public IMessageScenarios Add(string description, Func<dynamic> factory)
        {
            return this.Add(new Scenario(description, factory));
        }

        /// <summary>
        /// Add a message scenario
        /// </summary>
        /// <param name="description">Scenario description</param>
        /// <param name="metadata">Message metadata</param>
        /// <param name="factory">Message content factory</param>
        public IMessageScenarios Add(string description, dynamic metadata, Func<dynamic> factory)
        {
            return this.Add(new Scenario(description, factory, metadata));
        }
    }
}
