using System.Runtime.InteropServices;

namespace PactNet.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct PactHandle
    {
        public readonly ushort PactRef;
    }
}
