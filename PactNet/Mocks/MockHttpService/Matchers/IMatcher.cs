namespace PactNet.Mocks.MockHttpService.Matchers
{
    public interface IMatcher
    {
        string Match { get; }
        dynamic Example { get; }
    }
}