namespace PactNet.Verifier
{
    /// <summary>
    /// Arguments to the verifier FFI
    /// </summary>
    internal interface IVerifierArguments
    {
        /// <summary>
        /// Add an option with a value to the arguments
        /// </summary>
        /// <param name="option">Option name</param>
        /// <param name="value">Value</param>
        /// <param name="name">Name of the variable which supplied the value</param>
        /// <returns>Same verifier arguments</returns>
        VerifierArguments AddOption(string option, string value, string name = null);

        /// <summary>
        /// Add a flag to the arguments (i.e. an option with no value)
        /// </summary>
        /// <param name="flag">Flag name</param>
        /// <returns>Same verifier arguments</returns>
        VerifierArguments AddFlag(string flag);
    }
}
