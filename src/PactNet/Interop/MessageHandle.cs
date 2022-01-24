using System;
using System.Runtime.InteropServices;

namespace PactNet.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct MessageHandle
    {
        public readonly UIntPtr InteractionRef;
    }
}
