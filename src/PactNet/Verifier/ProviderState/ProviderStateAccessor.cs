using System;
using Provider.Tests;

namespace PactNet.Verifier.ProviderState
{
    public class ProviderStateAccessor : IProviderStateAccessor
    {
        /// <summary>
        /// Get a provider state at setup by description
        /// </summary>
        /// <returns>the provider state object</returns>
        public IStateHandler GetByDescriptionAndAction(string description, StateAction stateAction)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or empty");
            }

            return StateHandlers.GetByDescriptionAndAction(description, stateAction);
        }
    }
}
