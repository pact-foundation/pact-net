using System;
using System.Collections.Generic;
using Provider.Tests;

namespace PactNet.Verifier.ProviderState
{
    /// <summary>
    /// Message scenarios
    /// </summary>
    public class ProviderStatesFactory : IProviderStatesFactory
    {
        /// <summary>
        /// Add a configured provider stateHandler
        /// </summary>
        /// <param name="stateHandler">The provider stateHandler object</param>
        /// <returns>Fluent factory</returns>
        public IProviderStatesFactory Add(StateHandler stateHandler)
        {
            StateHandlers.AddStateHandler(stateHandler);
            return this;
        }

        /// <summary>
        /// Add a configured provider stateHandler
        /// </summary>
        /// <param name="stateHandler">The provider stateHandler object</param>
        /// <param name="stateAction">When the provider state is executed</param>
        /// <returns>Fluent factory</returns>
        public IProviderStatesFactory Add(StateHandler stateHandler, StateAction stateAction)
        {
            StateHandlers.AddStateHandler(stateHandler);
            return this;
        }

        /// <summary>
        /// Add a provider stateHandler
        /// </summary>
        /// <param name="description">provider stateHandler description</param>
        /// <param name="factory">provider stateHandler invoker</param>
        /// <returns>Fluent factory</returns>
        public IProviderStatesFactory Add(string description, Action factory)
        {
            this.Add(new StateHandler(description, factory));
            return this;
        }

        /// <summary>
        /// Add a provider stateHandler
        /// </summary>
        /// <param name="description">provider stateHandler description</param>
        /// <param name="factory">provider stateHandler invoker with args</param>
        /// <returns>Fluent factory</returns>
        public IProviderStatesFactory Add(string description, Action<IDictionary<string, string>> factory)
        {
            this.Add(new StateHandler(description, factory));
            return this;
        }

        /// <summary>
        /// Add a provider stateHandler
        /// </summary>
        /// <param name="description">provider stateHandler description</param>
        /// <param name="factory">provider stateHandler invoker</param>
        /// <param name="stateAction">When the provider state is executed</param>
        /// <returns>Fluent factory</returns>
        public IProviderStatesFactory Add(string description, Action factory, StateAction stateAction)
        {
            this.Add(new StateHandler(description, factory, stateAction));
            return this;
        }

        /// <summary>
        /// Add a provider stateHandler
        /// </summary>
        /// <param name="description">provider stateHandler description</param>
        /// <param name="factory">provider stateHandler invoker with args</param>
        /// <param name="stateAction">When the provider state is executed</param>
        /// <returns>Fluent factory</returns>
        public IProviderStatesFactory Add(string description, Action<IDictionary<string, string>> factory, StateAction stateAction)
        {
            this.Add(new StateHandler(description, factory, stateAction));
            return this;
        }
    }
}
