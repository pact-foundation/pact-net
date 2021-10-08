namespace PactNet.Verifier.ProviderState
{
    /// <summary>
    /// Interface to get the provider states from the core library
    /// </summary>
    public interface IProviderStateAccessor
    {
        /// <summary>
        /// Get a provider state by description
        /// </summary>
        /// <returns>the provider state object</returns>
        IProviderState GetByDescription(string description);
    }
}
