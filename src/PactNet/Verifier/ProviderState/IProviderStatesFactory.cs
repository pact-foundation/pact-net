using System;
using System.Collections.Generic;
using Provider.Tests;

namespace PactNet.Verifier.ProviderState
{
    /// <summary>
    /// The scenarios used to generate messages when verifying messaging pacts
    /// </summary>
    public interface IProviderStatesFactory
    {
        /// <summary>
        /// Add a configured provider stateHandler
        /// </summary>
        /// <param name="stateHandler">The provider stateHandler object</param>
        /// <returns>Fluent factory</returns>
        IProviderStatesFactory Add(StateHandler stateHandler);

        /// <summary>
        /// Add a configured provider stateHandler
        /// </summary>
        /// <param name="stateHandler">The provider stateHandler object</param>
        /// <param name="stateAction">When the provider state is executed</param>
        /// <returns>Fluent factory</returns>
        IProviderStatesFactory Add(StateHandler stateHandler, StateAction stateAction);

        /// <summary>
        /// Add a provider stateHandler
        /// </summary>
        /// <param name="description">provider stateHandler description</param>
        /// <param name="factory">provider stateHandler invoker</param>
        /// <returns>Fluent factory</returns>
        IProviderStatesFactory Add(string description, Action factory);

        /// <summary>
        /// Add a provider stateHandler
        /// </summary>
        /// <param name="description">provider stateHandler description</param>
        /// <param name="factory">provider stateHandler invoker</param>
        /// <returns>Fluent factory</returns>
        IProviderStatesFactory Add(string description, Action<IDictionary<string, string>> factory);

        /// <summary>
        /// Add a provider stateHandler
        /// </summary>
        /// <param name="description">provider stateHandler description</param>
        /// <param name="factory">provider stateHandler invoker</param>
        /// <param name="stateAction">When the provider state is executed</param>
        /// <returns>Fluent factory</returns>
        IProviderStatesFactory Add(string description, Action factory, StateAction stateAction);

        /// <summary>
        /// Add a provider stateHandler
        /// </summary>
        /// <param name="description">provider stateHandler description</param>
        /// <param name="factory">provider stateHandler invoker</param>
        /// <param name="stateAction">When the provider state is executed</param>
        /// <returns>Fluent factory</returns>
        IProviderStatesFactory Add(string description, Action<IDictionary<string, string>> factory, StateAction stateAction);
    }
}
