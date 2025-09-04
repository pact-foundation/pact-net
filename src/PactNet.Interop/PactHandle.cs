using System.Runtime.InteropServices;

namespace PactNet.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct PactHandle
    {
        public readonly ushort PactRef;
    }
}
