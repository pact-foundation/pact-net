using System.Collections.Generic;

namespace PactNet.Internal
{
    /// <summary>
    /// Extensions for <see cref="IDictionary{TKey,TValue}"/>
    /// </summary>
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Add an option with a value to the dictionary
        /// </summary>
        /// <param name="dict">Dictionary</param>
        /// <param name="option">Option to add</param>
        /// <param name="value">Value to add</param>
        /// <param name="name">Name of the original variable supplying the value</param>
        /// <returns>Same dictionary</returns>
        internal static IDictionary<string, string> AddOption(this IDictionary<string, string> dict, string option, string value, string name = null)
        {
            Guard.NotNullOrEmpty(value, name ?? nameof(value));

            dict[option] = value;

            return dict;
        }

        /// <summary>
        /// Add a flag (i.e. with no argument) to the dictionary
        /// </summary>
        /// <param name="dict">Dictionary</param>
        /// <param name="flag">Flag to add</param>
        /// <returns>Same dictionary</returns>
        internal static IDictionary<string, string> AddFlag(this IDictionary<string, string> dict, string flag)
        {
            dict[flag] = string.Empty;

            return dict;
        }
    }
}
