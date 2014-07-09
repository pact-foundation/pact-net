namespace PactNet.Models
{
    public class PactFile
    {
        public PactParty Provider { get; set; }
        public PactParty Consumer { get; set; }

        public dynamic Metadata { get; private set; }

        public PactFile()
        {
            Metadata = new
            {
                PactSpecificationVersion = "1.0.0"
            };
        }
    }
}