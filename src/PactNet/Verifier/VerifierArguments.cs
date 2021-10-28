using System;
using System.Collections.Generic;
using PactNet.Internal;

namespace PactNet.Verifier
{
    /// <summary>
    /// Represents the arguments passed to the verifier FFI
    /// </summary>
    internal class VerifierArguments : IVerifierArguments
    {
        private readonly ICollection<string> arguments;

        /// <summary>
        /// Initialises a new instance of the <see cref="VerifierArguments"/> class.
        /// </summary>
        public VerifierArguments()
        {
            this.arguments = new List<string>();
        }

        /// <summary>
        /// Add an option with a value to the arguments
        /// </summary>
        /// <param name="option">Option name</param>
        /// <param name="value">Value</param>
        /// <param name="name">Name of the variable which supplied the value</param>
        /// <returns>Same verifier arguments</returns>
        public VerifierArguments AddOption(string option, string value, string name = null)
        {
            Guard.NotNullOrEmpty(option, nameof(option));
            Guard.NotNullOrEmpty(value, name ?? nameof(value));
            
            this.arguments.Add(option);
            this.arguments.Add(value);

            return this;
        }

        /// <summary>
        /// Add a flag to the arguments (i.e. an option with no value)
        /// </summary>
        /// <param name="flag">Flag name</param>
        /// <returns>Same verifier arguments</returns>
        public VerifierArguments AddFlag(string flag)
        {
            Guard.NotNullOrEmpty(flag, nameof(flag));

            this.arguments.Add($"{flag}");
            return this;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Join(Environment.NewLine, this.arguments);
        }
    }
}
