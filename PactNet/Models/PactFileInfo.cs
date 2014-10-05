namespace PactNet.Models
{
    public struct PactFileInfo
    {
        public string Directory { get; private set; }

        public PactFileInfo(string directory) 
            : this()
        {
            Directory = directory;
        }
    }
}