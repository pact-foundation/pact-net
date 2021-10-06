using PactNet.Models;
using PactNet.Verifier;

namespace PactNet.Native.Verifier
{
    /// <summary>
    /// Configured the messaging scenarios
    /// </summary>
    internal class NativePactVerifierMessagingScenario : IPactVerifierMessagingScenario
    {
        private readonly IPactVerifierPair parentVerifier;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativePactVerifierMessagingScenario"/> class.
        /// </summary>
        /// <param name="parentVerifier">The parent verifier</param>
        public NativePactVerifierMessagingScenario(IPactVerifierPair parentVerifier)
        {
            this.parentVerifier = parentVerifier;
        }

        /// <summary>
        /// Add the messaging scenario
        /// </summary>
        /// <param name="scenario">the scenario object</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierMessagingScenario AddScenario(Scenario scenario)
        {
            Scenarios.AddScenario(scenario);
            return this;
        }

        /// <summary>
        /// Verify provider interactions
        /// </summary>
        public void Verify()
        {
            this.parentVerifier.Verify();
        }
    }
}
