using System;
using System.Collections.Generic;
using System.Linq;

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
        internal static List<Scenario> All = new List<Scenario>();

        /// <summary>
        /// Invokes the scenarios
        /// </summary>
        /// <param name="description">the name of the scenario</param>
        /// <returns>a dynamic message object</returns>
        public static dynamic InvokeScenario(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            return All.Single(x => x.Description == description).InvokeScenario();
        }

        /// <summary>
        /// Checks if a scenario exists
        /// </summary>
        /// <param name="description">the scenario description</param>
        /// <returns>If the scenario exists</returns>
        public static bool Exist(string description)
        {
            return All.Any(x => x.Description == description);
        }

        /// <summary>
        /// Get a scenario by description 
        /// </summary>
        /// <param name="description">the scenario description</param>
        /// <returns>The scenario</returns>
        public static Scenario GetByDescription(string description)
        {
            return All.Single(x => x.Description == description);
        }
    }
}
