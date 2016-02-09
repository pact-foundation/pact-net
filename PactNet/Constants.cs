namespace PactNet
{
    internal static class Constants
    {
        public const string AdministrativeRequestHeaderKey = "X-Pact-Mock-Service";
        public const string AdministrativeRequestTestContextHeaderKey = "X-Test-Context";
        public const string InteractionsPath = "/interactions";
        public const string InteractionsVerificationPath = "/interactions/verification";
        public const string PactPath = "/pact";
        public const string DefaultPactDir = @"..\..\pacts\";
        public const string DefaultLogDir = @"..\..\logs\";
    }
}