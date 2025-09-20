using PactNet.Exceptions;
using PactNet.Interop;

namespace PactNet.Drivers.Plugins
{
    /// <summary>
    /// Driver for plugin interactions
    /// </summary>
    internal class PluginInteractionDriver : IPluginInteractionDriver
    {
        private readonly PactHandle pact;
        private readonly InteractionHandle interaction;

        /// <summary>
        /// Initialises a new instance of the <see cref="PluginInteractionDriver"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        /// <param name="interaction">Interaction handle</param>
        public PluginInteractionDriver(PactHandle pact, InteractionHandle interaction)
        {
            this.pact = pact;
            this.interaction = interaction;
        }

        /// <summary>
        /// Add a provider state to the interaction
        /// </summary>
        /// <param name="description">Provider state description</param>
        public void Given(string description)
            => NativeInterop.Given(this.interaction, description).CheckInteropSuccess();

        /// <summary>
        /// Add a provider state with a parameter to the interaction
        /// </summary>
        /// <param name="description">Provider state description</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        public void GivenWithParam(string description, string name, string value)
            => NativeInterop.GivenWithParam(this.interaction, description, name, value).CheckInteropSuccess();

        /// <summary>
        /// Add a plugin interaction content
        /// </summary>
        /// <param name="contentType">Content type</param>
        /// <param name="content">Content</param>
        public void WithContent(string contentType, string content)
        {
            uint code = NativeInterop.InteractionContents(this.interaction, InteractionPart.Request, contentType, content);

            if (code != 0)
            {
                throw code switch
                {
                    1 => new PactFailureException("A general panic was caught"),
                    2 => new PactFailureException("The mock server has already been started"),
                    3 => new PactFailureException("The interaction handle is invalid"),
                    4 => new PactFailureException("The content type is not valid"),
                    5 => new PactFailureException("The contents JSON is not valid JSON"),
                    6 => new PactFailureException("The plugin returned an error"),
                    _ => new PactFailureException($"An unknown error occurred when setting the interaction contents: {code}"),
                };
            }
        }
    }
}
