using System.IO;

namespace PactNet
{
    internal static class Constants
    {
        public const string AdministrativeRequestHeaderKey = "X-Pact-Mock-Service";
        public const string InteractionsPath = "/interactions";
        public const string InteractionsVerificationPath = "/interactions/verification";
        public const string PactPath = "/pact";
        public static string DefaultPactDir = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}";
        public static string DefaultLogDir = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}logs{Path.DirectorySeparatorChar}";
    }
}