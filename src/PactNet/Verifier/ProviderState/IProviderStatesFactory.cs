using System;

namespace PactNet.Verifier.ProviderState
{
    /// <summary>
    /// The scenarios used to generate messages when verifying messaging pacts
    /// </summary>
    public interface IProviderStatesFactory
    {
        /// <summary>
        /// Add a configured provider state
        /// </summary>
        /// <param name="providerState">The provider state object</param>
        /// <returns>Fluent factory</returns>
        IProviderStatesFactory Add(ProviderState providerState);

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="description">provider state description</param>
        /// <param name="factory">provider state invoker</param>
        /// <returns>Fluent factory</returns>
        IProviderStatesFactory Add(string description, Action factory);

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="description">provider state description</param>
        /// <param name="factory">provider state invoker</param>
        /// <param name="args">the arguments of the invoker method</param>
        /// <returns>Fluent factory</returns>
        IProviderStatesFactory Add(string description, Action factory, params object[] args);
    }
}
