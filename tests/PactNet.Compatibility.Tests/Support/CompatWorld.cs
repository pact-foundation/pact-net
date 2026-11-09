using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PactNet.Compatibility.Tests.Support
{
    /// <summary>
    /// Raised by a step when the scenario's expectation does not hold
    /// </summary>
    public sealed class CompatAssertionException : Exception
    {
        public CompatAssertionException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Per-scenario state shared between step classes (injected by Reqnroll)
    /// </summary>
    public sealed class CompatWorld
    {
        internal List<InteractionDefinition> Interactions { get; set; } = new();

        internal MockServerSession Session { get; set; }

        internal HttpResponse Response { get; set; }

        /// <summary>
        /// First step failure; once set, remaining steps are skipped and the ratchet decides the outcome
        /// </summary>
        public Exception Failure { get; private set; }

        /// <summary>
        /// Set when the scenario uses something this runner can't express (counts as unsupported, not failed)
        /// </summary>
        public Exception Unsupported { get; private set; }

        public string FailedStep { get; private set; }

        public void Run(string step, Action body) =>
            this.RunAsync(step, () =>
            {
                body();
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

        public async Task RunAsync(string step, Func<Task> body)
        {
            if (this.Failure != null || this.Unsupported != null)
            {
                return;
            }

            try
            {
                await body();
            }
            catch (NotSupportedException e)
            {
                this.Unsupported = e;
                this.FailedStep = step;
            }
            catch (Exception e)
            {
                this.Failure = e;
                this.FailedStep = step;
            }
        }

        internal InteractionDefinition Interaction(int no)
        {
            InteractionDefinition found = this.Interactions.Find(i => i.No == no);
            return found ?? throw new InvalidOperationException($"Interaction {no} has not been defined");
        }

        internal MockServerSession RequireSession() =>
            this.Session ?? throw new InvalidOperationException("The mock server has not been started");

        public static void Check(bool condition, string message)
        {
            if (!condition)
            {
                throw new CompatAssertionException(message);
            }
        }
    }
}
