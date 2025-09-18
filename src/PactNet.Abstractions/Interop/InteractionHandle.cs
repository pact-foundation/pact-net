using System.Runtime.InteropServices;

namespace PactNet.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct InteractionHandle
    {
        public readonly uint InteractionRef;
    }
}
