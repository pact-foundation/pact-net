using PactNet.Mocks.MockHttpService;

namespace PactNet
{
    public class PactConfig
    {
        public string PactDir { get; set; }
        public string LogDir { get; set; }

        public PactConfig()
        {
            PactDir = Constants.DefaultPactDir;
            LogDir = Constants.DefaultLogDir;
        }
    }
}