namespace PactNet.Core
{
    internal interface IPactProcessConfiguration
    {
        string Path { get; }
        string Arguments { get; }
        bool WaitForExit { get; }
    }
}