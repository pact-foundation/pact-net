using System;
using System.Runtime.InteropServices;

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

        /// <summary>
        /// Ensures a condition is true
        /// </summary>
        /// <param name="value">Condition to check</param>
        /// <param name="message">Message for failed condition</param>
        /// <exception cref="InvalidOleVariantTypeException">Condition was not met</exception>
        internal static void That(bool value, string message)
        {
            if (!value)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
