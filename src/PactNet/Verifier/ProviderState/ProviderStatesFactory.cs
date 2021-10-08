using System;

namespace PactNet.Verifier.ProviderState
{
    /// <summary>
    /// Message scenarios
    /// </summary>
    public class ProviderStatesFactory : IProviderStatesFactory
    {
        /// <summary>
        /// Add a configured provider state
        /// </summary>
        /// <param name="providerState">The provider state object</param>
        /// <returns>Fluent factory</returns>
        public IProviderStatesFactory Add(ProviderState providerState)
        {
            ProviderStates.AddProviderState(providerState);
            return this;
        }

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="description">provider state description</param>
        /// <param name="factory">provider state invoker</param>
        /// <returns>Fluent factory</returns>
        public IProviderStatesFactory Add(string description, Action factory)
        {
            this.Add(new ProviderState(description, factory));
            return this;
        }

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="description">provider state description</param>
        /// <param name="factory">provider state invoker</param>
        /// <param name="args">the arguments of the invoker method</param>
        /// <returns>Fluent factory</returns>
        public IProviderStatesFactory Add(string description, Action factory, params object[] args)
        {
            this.Add(new ProviderState(description, factory, args));
            return this;
        }
    }
}
