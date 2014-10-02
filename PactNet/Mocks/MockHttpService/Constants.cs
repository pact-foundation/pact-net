namespace PactNet.Mocks.MockHttpService
{
    public static class Constants
    {
        public const string AdministrativeRequestHeaderKey = "X-Pact-Mock-Service";
        public const string InteractionsPath = "/interactions";
        public const string InteractionsVerificationPath = "/interactions/verification";
        public const string PactPath = "/pact";
        public const string PactFileDirectory = "../../pacts/";
    }
}