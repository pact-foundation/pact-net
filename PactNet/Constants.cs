using System.IO;

namespace PactNet
{
    internal static class Constants
    {
        public const string AdministrativeRequestHeaderKey = "X-Pact-Mock-Service";
        public const string InteractionsPath = "/interactions";
        public const string InteractionsVerificationPath = "/interactions/verification";
        public const string PactPath = "/pact";
        public static string DefaultPactDir = Path.GetFullPath(@"..\..\pacts\");
        public static string DefaultLogDir = Path.GetFullPath(@"..\..\logs\");
    }
}