using System;
using System.Collections.Generic;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// The scenarios used to generate messages when verifying messaging pacts
    /// </summary>
    public interface IMessageScenarios
    {
        /// <summary>
        /// Configured scenarios
        /// </summary>
        IReadOnlyDictionary<string, Scenario> Scenarios { get; }

        /// <summary>
        /// Add a message scenario
        /// </summary>
        /// <param name="description">Scenario description</param>
        /// <param name="factory">Message content factory</param>
        IMessageScenarios Add(string description, Func<dynamic> factory);

        /// <summary>
        /// Add a message scenario by configuring a scenario builder
        /// </summary>
        /// <param name="description">Scenario description</param>
        /// <param name="configure">Scenario configure</param>
        /// <returns>Fluent builder</returns>
        IMessageScenarios Add(string description, Action<IMessageScenarioBuilder> configure);
    }
}
