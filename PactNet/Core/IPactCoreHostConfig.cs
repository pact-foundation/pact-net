namespace PactNet.Core
{
    internal interface IPactCoreHostConfig
    {
        string Path { get; }
        string Arguments { get; }
        bool WaitForExit { get; }
    }
}