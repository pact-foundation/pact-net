using System;
using System.Runtime.InteropServices;
namespace PactNet.Interop
{
    /// <summary>
    /// Represents the result of a string operation.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct StringResult
    {
        /// <summary>
        /// The possible tag values.
        /// </summary>
        public enum Tag
        {
            /// <summary>
            /// Indicates that the string operation succeeded.
            /// </summary>
            StringResult_Ok,

            /// <summary>
            /// Indicates that the string operation failed.
            /// </summary>
            StringResult_Failed,
        };

        /// <summary>
        /// The tag value
        /// </summary>
        [FieldOffset(0)]
        public Tag tag;

        /// <summary>
        /// The body of the StringResult when the tag value is StringResult_Ok
        /// </summary>
        [FieldOffset(8)]
        public StringResult_Ok_Body ok;

        /// <summary>
        /// The body of the StringResult when the tag value is StringResult_Failed
        /// </summary>
        [FieldOffset(8)]
        public StringResult_Failed_Body failed;
    }
    /// <summary>
    /// The OK body
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct StringResult_Ok_Body
    {
        /// <summary>
        /// A pointer to the string result
        /// </summary>
        public IntPtr _0;
    }

    /// <summary>
    /// The failed Body
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct StringResult_Failed_Body
    {
        /// <summary>
        /// A pointer to the error message.
        /// </summary>
        public IntPtr _0;
    }
}
