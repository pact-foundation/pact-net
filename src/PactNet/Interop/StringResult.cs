using System;
using System.Runtime.InteropServices;
namespace PactNet.Interop
{
    [StructLayout(LayoutKind.Explicit)]
    public struct StringResult
    {
        public enum Tag
        {
            StringResult_Ok,
            StringResult_Failed,
        };

        [FieldOffset(0)]
        public Tag tag;

        [FieldOffset(8)]
        public StringResult_Ok_Body ok;

        [FieldOffset(8)]
        public StringResult_Failed_Body failed;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StringResult_Ok_Body
    {
        public IntPtr _0;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StringResult_Failed_Body
    {
        public IntPtr _0;
    }

    

}