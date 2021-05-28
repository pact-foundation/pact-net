namespace PactNet.Matchers
{
    public interface IMatcher
    {
        string Match { get; }
        dynamic Example { get; }
    }
}