using System;
using System.Collections.Generic;
using System.Linq;

namespace PactNet.Verifier.ProviderState
{
    /// <summary>
    /// Defines the providerStates static list for messaging support
    /// </summary>
    public static class ProviderStates
    {
        /// <summary>
        /// The available providerStates
        /// </summary>
        public static readonly List<ProviderState> AllProviderStates = new List<ProviderState>();

        /// <summary>
        /// Number of providerStates
        /// </summary>
        public static int NumberOfStates => AllProviderStates.Count;

        /// <summary>
        /// Add a state
        /// </summary>
        /// <param name="state">the state to add</param>
        internal static void AddProviderState(ProviderState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (Exist(state.Description))
            {
                throw new InvalidOperationException($"Scenario \"{state.Description}\" already added");
            }

            AllProviderStates.Add(state);
        }

        /// <summary>
        /// Add multiple providerStates
        /// </summary>
        /// <param name="providerStates">the state list to add</param>
        internal static void AddScenarios(IReadOnlyCollection<ProviderState> providerStates)
        {
            if (providerStates == null || providerStates.Any() == false)
            {
                throw new ArgumentException("providerStates cannot be null or empty");
            }

            if (providerStates.Any(x => Exist(x.Description)))
            {
                throw new InvalidOperationException("A state has already been added");
            }

            AllProviderStates.AddRange(providerStates);
        }

        /// <summary>
        /// Executes the provider state
        /// </summary>
        /// <param name="description">the name of the state</param>
        /// <returns>a dynamic message object</returns>
        internal static void Execute(string description)
        {
            var providerStateToExecute = AllProviderStates.FirstOrDefault(x => x.Description == description);

            if (providerStateToExecute == null)
            {
                throw new InvalidOperationException($"Provider state \"{description}\" not found. You need to add the provider state first");
            }

            providerStateToExecute.Execute();
        }

        /// <summary>
        /// Checks if a state exists
        /// </summary>
        /// <param name="description">the state description</param>
        /// <returns>If the state exists</returns>
        private static bool Exist(string description)
        {
            return AllProviderStates.Any(x => x.Description == description);
        }

        /// <summary>
        /// Get a provider state by state description 
        /// </summary>
        /// <param name="description">the description</param>
        /// <returns>The state</returns>
        internal static ProviderState GetByDescription(string description)
        {
            var providerState = AllProviderStates.FirstOrDefault(x => x.Description == description);

            return providerState;
        }

        /// <summary>
        /// Clear all providerStates
        /// </summary>
        internal static void ClearProviderStates()
        {
            AllProviderStates.Clear();
        }
    }
}
