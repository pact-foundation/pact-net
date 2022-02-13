using System;
using System.Threading.Tasks;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// The scenarios used to generate messages when verifying messaging pacts
    /// </summary>
    public interface IMessageScenarios
    {
        /// <summary>
        /// Add a message scenario
        /// </summary>
        /// <param name="description">Scenario description</param>
        /// <param name="factory">Message content factory</param>
        IMessageScenarios Add(string description, Func<dynamic> factory);

        /// <summary>
        /// Add a message scenario
        /// </summary>
        /// <param name="description">Scenario description</param>
        /// <param name="configure">Scenario configure</param>
        /// <returns></returns>
        IMessageScenarios Add(string description, Action<IMessageScenarioBuilder> configure);

        /// <summary>
        /// Add a message scenario
        /// </summary>
        /// <param name="description">Scenario description</param>
        /// <param name="configure">Scenario configure</param>
        /// <returns></returns>
        IMessageScenarios Add(string description, Func<IMessageScenarioBuilder, Task> configure);
    }
}
