namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Interface to get the provider states from the core library
    /// </summary>
    public interface IMessagingScenarioAccessor
    {
        /// <summary>
        /// Get a provider state by description
        /// </summary>
        /// <returns>the provider state object</returns>
        IScenario GetByDescription(string description);
    }
}
