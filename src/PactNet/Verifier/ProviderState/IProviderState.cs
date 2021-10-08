namespace PactNet.Verifier.ProviderState
{
    public interface IProviderState
    {
        /// <summary>
        /// The provider state description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Invoke a provider state
        /// </summary>
        void Execute();
    }
}
