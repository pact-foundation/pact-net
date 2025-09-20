namespace PactNet.Drivers.Plugins
{
    /// <summary>
    /// Driver for plugin interactions
    /// </summary>
    internal interface IPluginInteractionDriver : IProviderStateDriver
    {
        /// <summary>
        /// Add a plugin interaction content
        /// </summary>
        /// <param name="contentType">Content type</param>
        /// <param name="content">Content</param>
        void WithContent(string contentType, string content);
    }
}
