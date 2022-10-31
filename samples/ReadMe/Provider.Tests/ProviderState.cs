namespace ReadMe.Provider.Tests
{
    public class ProviderState
    {
        public ProviderStateAction Action { get; set; }
        public string State { get; set; }
    }

    public enum ProviderStateAction
    {
        Setup, Teardown
    }
}
