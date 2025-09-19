using System;

namespace PactNet.Interop.Drivers;

internal class PluginDriver(PactHandle pact) : IPluginDriver
{
    public void WithInteractionContents(InteractionHandle interaction, string contentType, string content, InteractionPart part)
    {
        uint result = PluginInterop.PluginInteractionContents(interaction, part, contentType, content);
        if (result > 0)
        {
            throw result switch
            {
                1 => new InvalidOperationException("The pact reference library panicked."),
                2 => new InvalidOperationException("The mock server has already been started."),
                3 => new InvalidOperationException("The interaction handle is invalid."),
                4 => new InvalidOperationException("TThe content type is not valid."),
                5 => new InvalidOperationException("The contents JSON is not valid JSON."),
                6 => new InvalidOperationException("The plugin returned an error."),
                _ => new InvalidOperationException($"Unknown mock server error: {result}")
            };
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
    /// </summary>
    ~PluginDriver()
    {
        this.ReleaseUnmanagedResources();
    }

    /// <summary>
    /// Release unmanaged resources
    /// </summary>
    private void ReleaseUnmanagedResources()
    {
        PluginInterop.PluginCleanup(pact);
    }
}
