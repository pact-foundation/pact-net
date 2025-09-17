using System;
using PactNet.Drivers;

namespace PactNet.Interop;

internal class PluginDriver(PactHandle pact) : IPluginDriver
{
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
