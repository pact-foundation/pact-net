using System;

namespace PactNet.Native
{
    /// <summary>
    /// Defines the scenario model
    /// </summary>
    public class Scenario
    {
        /// <summary>
        /// The description of the scenario
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The metadata
        /// </summary>
        public dynamic Metadata { get; set; }

        /// <summary>
        /// The invoker that will publish the content
        /// </summary>
        public Func<dynamic> Invoker { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="Scenario"/>
        /// </summary>
        /// <param name="description">the scenario description</param>
        /// <param name="invoker">the action invoking the content</param>
        public Scenario(string description, Func<dynamic> invoker)
        {
            this.Description = description;
            this.Invoker = invoker;
        }

        /// <summary>
        /// Creates an instance of <see cref="Scenario"/>
        /// </summary>
        /// <param name="description">the scenario description</param>
        /// <param name="invoker">the action invoking the content</param>
        /// <param name="metadata">the metadata</param>
        public Scenario(string description, Func<dynamic> invoker, dynamic metadata)
            : this(description, invoker)
        {
            this.Metadata = metadata;
        }

        /// <summary>
        /// Invoke a scenario
        /// </summary>
        /// <returns>The scenario message content</returns>
        public dynamic InvokeScenario()
        {
            if (Invoker == null)
            {
                throw new InvalidOperationException("The scenario invoker needs to be set before executing it");
            }

            return Invoker.Invoke();
        }
    }
}
