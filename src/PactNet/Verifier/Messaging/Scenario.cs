using System;

namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// Defines the scenario model
    /// </summary>
    public class Scenario
    {
        /// <summary>
        /// The description of the scenario
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The invoker that will publish the content
        /// </summary>
        private readonly Func<dynamic> invoker;

        /// <summary>
        /// The metadata
        /// </summary>
        public dynamic Metadata { get; }

        /// <summary>
        /// Creates an instance of <see cref="Scenario"/>
        /// </summary>
        /// <param name="description">the scenario description</param>
        /// <param name="invoker">the action invoking the content</param>
        public Scenario(string description, Func<dynamic> invoker)
        {
            this.Description = !string.IsNullOrWhiteSpace(description) ? description : throw new ArgumentException("Description cannot be null or empty");
            this.invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
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
            return invoker.Invoke();
        }
    }
}
