using System;

namespace PactNet.Verifier.ProviderState
{
    public class ProviderStateAccessor : IProviderStateAccessor
    {
        /// <summary>
        /// Get a provider state by description
        /// </summary>
        /// <returns>the provider state object</returns>
        public IProviderState GetByDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or empty");
            }

            return ProviderStates.GetByDescription(description);
        }
    }
}
