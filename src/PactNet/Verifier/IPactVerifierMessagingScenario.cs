using PactNet.Models;

namespace PactNet.Verifier
{
    /// <summary>
    /// Configured the messaging scenarios
    /// </summary>
    public interface IPactVerifierMessagingScenario
    {
        /// <summary>
        /// Add the messaging scenarios
        /// </summary>
        /// <param name="scenarios">the list of scenarios</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierMessagingScenario AddScenario(Scenario scenarios);

        /// <summary>
        /// Verify provider interactions
        /// </summary>
        void Verify();
    }
}
