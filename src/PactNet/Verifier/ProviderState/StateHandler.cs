using System;
using System.Collections.Generic;
using PactNet.Exceptions;
using Provider.Tests;

namespace PactNet.Verifier.ProviderState
{
    /// <summary>
    /// Defines the provider description model
    /// </summary>
    public class StateHandler : IStateHandler
    {
        /// <summary>
        /// The provider state description
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// When the provider state is executed
        /// </summary>
        public StateAction Action { get; }

        //public IStateHandler SetParams(IDictionary<string, string> args)
        //{
        //    this.args = args;
        //    return this;
        //}

        /// <summary>
        /// The invoker that will execute the provider state
        /// </summary>
        private readonly Action invoker;

        /// <summary>
        /// The invoker that will execute the provider state
        /// </summary>
        private Action<IDictionary<string, string>> invokerWithArgs;

        ///// <summary>
        ///// Arguments
        ///// </summary>
        //private IDictionary<string, string> args;

        /// <summary>
        /// Creates an instance of <see cref="StateHandler"/>
        /// </summary>
        /// <param name="description">the description</param>
        /// <param name="invoker">the action invoking the content</param>
        public StateHandler(string description, Action invoker) : this(description, invoker, null)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="StateHandler"/>
        /// </summary>
        /// <param name="description">the description</param>
        /// <param name="invokerWithArgs">the action invoking the content with args</param>
        public StateHandler(string description, Action<IDictionary<string, string>> invokerWithArgs) : this(description, null, invokerWithArgs)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="StateHandler"/>
        /// </summary>
        /// <param name="description">the description</param>
        /// <param name="invoker">the action invoking the content</param>
        /// <param name="stateAction">When the provider state is executed</param>
        public StateHandler(string description, Action invoker, StateAction stateAction) : this(description, invoker, null, stateAction)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="StateHandler"/>
        /// </summary>
        /// <param name="description">the description</param>
        /// <param name="invokerWithArgs">the action invoking the content</param>
        /// <param name="stateAction">When the provider state is executed</param>
        public StateHandler(string description, Action<IDictionary<string, string>> invokerWithArgs, StateAction stateAction) : this(description, null, invokerWithArgs, stateAction)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="StateHandler"/>
        /// </summary>
        /// <param name="description">the description</param>
        /// <param name="invoker">the action invoking the content</param>
        /// <param name="invokerWithArgs">the action invoking the content with args</param>
        public StateHandler(string description, Action invoker, Action<IDictionary<string, string>> invokerWithArgs) : this(description, invoker, invokerWithArgs, StateAction.Setup)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="StateHandler"/>
        /// </summary>
        /// <param name="description">the description</param>
        /// <param name="invoker">the action invoking the content</param>
        /// <param name="invokerWithArgs">the action invoking the content with args</param>
        /// <param name="stateAction">When the provider state is executed</param>
        public StateHandler(string description, Action invoker, Action<IDictionary<string, string>> invokerWithArgs, StateAction stateAction)
        {
            this.Description = !string.IsNullOrWhiteSpace(description) ? description : throw new ArgumentException("Description cannot be null or empty");
            this.invoker = invoker;
            this.invokerWithArgs = invokerWithArgs;
            this.Action = stateAction;
        }

        /// <summary>
        /// Invoke a provider state
        /// </summary>
        public void Execute()
        {
            if (invoker == null)
            {
                throw new StateHandlerExecutionException();
            }

            this.invoker.Invoke();
        }

        /// <summary>
        /// Invoke a provider state
        /// </summary>
        public void Execute(IDictionary<string, string> args)
        {
            if (invokerWithArgs == null)
            {
                throw new StateHandlerExecutionException();
            }

            this.invokerWithArgs.Invoke(args);
        }
    }
}
