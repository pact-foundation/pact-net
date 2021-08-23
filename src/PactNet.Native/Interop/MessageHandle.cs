using System;
using System.Runtime.InteropServices;

namespace PactNet.Native.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct MessageHandle
    {
        public readonly UIntPtr Pact;
        public readonly UIntPtr Interaction;
    }
}
