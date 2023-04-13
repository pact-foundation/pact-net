using System;
using System.Runtime.InteropServices;
namespace PactNet.Interop

{  
    [StructLayout(LayoutKind.Explicit)]
    internal struct StringResult
    {
        public enum Tag
        {
            StringResult_Ok,
            StringResult_Failed,
        };

        [FieldOffset(0)]
        public Tag tag;

        [FieldOffset(8)]
        public StringResultOkBody ok;

        [FieldOffset(8)]
        public StringResultFailedBody failed;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct StringResultOkBody
    {
        public IntPtr successPointer;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct StringResultFailedBody
    {
        public IntPtr errorPointer;
    }
}
