using System.Collections.Generic;
using Provider.Tests;

namespace PactNet.Verifier.ProviderState
{
    public interface IStateHandler
    {
        /// <summary>
        /// The provider state description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// When the provider state is executed
        /// </summary>
        StateAction Action { get; }

        /// <summary>
        /// Invoke a provider state
        /// </summary>
        void Execute();

        /// <summary>
        /// Invoke a provider state
        /// </summary>
        void Execute(IDictionary<string, string> args);
    }
}
