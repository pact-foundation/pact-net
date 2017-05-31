namespace PactNet
{
    public class PactVerifierConfig
    {
        public string LogDir { get; set; }

        //TODO: Do we still want do have this?
        //public IList<IReportOutputter> ReportOutputters { get; private set; }

        internal string LoggerName;

        public PactVerifierConfig()
        {
            LogDir = Constants.DefaultLogDir;
            /*ReportOutputters = new List<IReportOutputter>
            {
                new ConsoleReportOutputter(),
                new FileReportOutputter(() => LoggerName)
            };*/
        }
    }
}