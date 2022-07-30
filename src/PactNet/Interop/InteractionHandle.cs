using System.Runtime.InteropServices;

namespace PactNet.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct InteractionHandle
    {
        public readonly uint InteractionRef;
    }
}
