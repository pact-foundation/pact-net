namespace PactNet.Comparers
{
    public class ReportedResult
    {
        public OutputType OutputType { get; set; }
        public string Message { get; set; }

        public ReportedResult(OutputType outputType, string message)
        {
            OutputType = outputType;
            Message = message;
        }
    }
}