using System;

namespace PactNet.Internal
{
    /// <summary>
    /// Guard methods
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Ensures a string is not null or empty
        /// </summary>
        /// <param name="value">String value to check</param>
        /// <param name="name">Original variable name</param>
        /// <exception cref="ArgumentException">Value was null or empty</exception>
        internal static void NotNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value must not be null or empty", name);
            }
        }

        /// <summary>
        /// Ensures a value is not null
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="name">Original variable name</param>
        internal static void NotNull(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentException("Value must not be null", name);
            }
        }
    }
}
