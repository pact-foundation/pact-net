namespace PactNet.Verifier.ProviderState
{
    /// <summary>
    /// Interface to get the provider states from the core library
    /// </summary>
    public interface IProviderStateAccessor
    {
        /// <summary>
        /// Get a provider state at setup by description
        /// </summary>
        /// <returns>the provider state object</returns>
        IStateHandler GetByDescriptionAndAction(string description, StateAction stateAction);
    }
}
