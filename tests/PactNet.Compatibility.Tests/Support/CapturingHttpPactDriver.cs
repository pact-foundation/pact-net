using PactNet.Drivers;
using PactNet.Interop;

namespace PactNet.Compatibility.Tests.Support
{
    /// <summary>
    /// HTTP pact driver that remembers the handle of the most recently created interaction, so that
    /// parts the DSL can't express (binary bodies) can be added through the FFI directly
    /// </summary>
    internal sealed class CapturingHttpPactDriver : IHttpPactDriver
    {
        private readonly PactHandle pact;
        private readonly HttpPactDriver inner;

        public CapturingHttpPactDriver(PactHandle pact)
        {
            this.pact = pact;
            this.inner = new HttpPactDriver(pact);
        }

        public InteractionHandle LastInteraction { get; private set; }

        public IHttpInteractionDriver NewHttpInteraction(string description)
        {
            this.LastInteraction = NativeInterop.NewInteraction(this.pact, description);
            return new HttpInteractionDriver(this.pact, this.LastInteraction);
        }

        public IMockServerDriver CreateMockServer(string host, int? port, bool tls)
            => this.inner.CreateMockServer(host, port, tls);

        public void WritePactFile(string directory)
            => this.inner.WritePactFile(directory);
    }
}
