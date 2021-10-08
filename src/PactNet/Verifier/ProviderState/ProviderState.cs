using System;

namespace PactNet.Verifier.ProviderState
{
    /// <summary>
    /// Defines the provider description model
    /// </summary>
    public class ProviderState : IProviderState
    {
        /// <summary>
        /// The provider state description
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The invoker that will execute the provider state
        /// </summary>
        private readonly Action invoker;

        /// <summary>
        /// Arguments to add to the invoker
        /// </summary>
        private readonly object[] args;

        /// <summary>
        /// Creates an instance of <see cref="ProviderState"/>
        /// </summary>
        /// <param name="description">the description</param>
        /// <param name="invoker">the action invoking the content</param>
        /// <param name="args">the arguments of the invoker</param>
        public ProviderState(string description, Action invoker, params object[] args)
        {
            this.Description = !string.IsNullOrWhiteSpace(description) ? description : throw new ArgumentException("Description cannot be null or empty");
            this.invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
            this.args = args;
        }

        /// <summary>
        /// Invoke a provider state
        /// </summary>
        public void Execute()
        {
            if (args == null)
            {
                this.invoker.Invoke();
            }

            this.invoker.DynamicInvoke(this.args);
        }
    }
}
