using System;
using System.Runtime.InteropServices;

namespace PactNet.Native.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct PactHandle
    {
        public readonly UIntPtr Pact;
    }
}
