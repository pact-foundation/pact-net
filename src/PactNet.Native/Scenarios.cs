using System;
using System.Collections.Generic;
using System.Linq;
using PactNet.Models;

namespace PactNet.Native
{
    /// <summary>
    /// Defines the scenarios static list for messaging support
    /// </summary>
    public class Scenarios
    {
        /// <summary>
        /// The available scenarios
        /// </summary>
        internal readonly List<Scenario> AllScenarios = new List<Scenario>();

        /// <summary>
        /// Add a scenario
        /// </summary>
        /// <param name="scenario">the scenario to add</param>
        public void AddScenario(Scenario scenario)
        {
            if (scenario == null)
            {
                throw new ArgumentNullException(nameof(scenario));
            }

            if (Exist(scenario.Description))
            {
                throw new InvalidOperationException($"Scenario \"{scenario.Description}\" already added");
            }

            AllScenarios.Add(scenario);
        }

        /// <summary>
        /// Add multiple scenarios
        /// </summary>
        /// <param name="scenarios">the scenario list to add</param>
        public void AddScenarios(IReadOnlyCollection<Scenario> scenarios)
        {
            if (scenarios == null || scenarios.Any() == false)
            {
                throw new ArgumentException("scenarios cannot be null or empty");
            }

            if (scenarios.Any(x => Exist(x.Description)))
            {
                throw new InvalidOperationException("A scenario has already been added");
            }

            AllScenarios.AddRange(scenarios);
        }

        /// <summary>
        /// Invokes the scenarios
        /// </summary>
        /// <param name="description">the name of the scenario</param>
        /// <returns>a dynamic message object</returns>
        public dynamic InvokeScenario(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            var scenarioToInvoke = AllScenarios.FirstOrDefault(x => x.Description == description);

            if (scenarioToInvoke == null)
            {
                throw new InvalidOperationException($"Scenario \"{description}\" not found. You need to add the scenario first");
            }

            return scenarioToInvoke.InvokeScenario();
        }

        /// <summary>
        /// Checks if a scenario exists
        /// </summary>
        /// <param name="description">the scenario description</param>
        /// <returns>If the scenario exists</returns>
        public bool Exist(string description)
        {
            return AllScenarios.Any(x => x.Description == description);
        }

        /// <summary>
        /// Get a scenario by description 
        /// </summary>
        /// <param name="description">the scenario description</param>
        /// <returns>The scenario</returns>
        public Scenario GetByDescription(string description)
        {
            var scenario = AllScenarios.FirstOrDefault(x => x.Description == description);

            return scenario ?? throw new InvalidOperationException($"Scenario \"{description}\" not found. You need to add the scenario first");
        }

        /// <summary>
        /// Clear all scenarios
        /// </summary>
        public void ClearScenarios()
        {
            AllScenarios.Clear();
        }
    }
}
