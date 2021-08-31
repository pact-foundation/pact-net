using System;
using System.Runtime.InteropServices;

namespace PactNet.Native.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct MessagePactHandle
    {
        public readonly UIntPtr Pact;
    }
}
