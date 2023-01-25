namespace ReadMe.Provider.Tests
{
    public class ProviderState
    {
        public enum StateAction
        {
            Setup, Teardown
        }

        public string State { get; set; }
        public StateAction Action { get; set; }
    }
}
