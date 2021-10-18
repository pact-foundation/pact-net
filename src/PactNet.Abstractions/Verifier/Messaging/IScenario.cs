namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Defines the scenario interface
    /// </summary>
    public interface IScenario
    {
        /// <summary>
        /// The description of the scenario
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The metadata
        /// </summary>
        dynamic Metadata { get; }

        /// <summary>
        /// Invoke a scenario
        /// </summary>
        /// <returns>The scenario message content</returns>
        dynamic InvokeScenario();
    }
}
