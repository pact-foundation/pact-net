using System;

namespace PactNet.Interop.Drivers;

internal interface IPluginDriver : IDisposable
{
    /// <summary>
    /// Add interaction contents for a plugin interaction.
    /// </summary>
    /// <param name="interaction"></param>
    /// <param name="contentType"></param>
    /// <param name="content"></param>
    /// <param name="part"></param>
    /// <exception cref="InvalidOperationException"></exception>
    void WithInteractionContents(InteractionHandle interaction, string contentType, string content, InteractionPart part = InteractionPart.Request);
}
