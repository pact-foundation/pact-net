namespace PactNet.PactVerification
{
    public interface IMessageInvoker
    {
        string Invoke(PactMessageDescription messageDescription);
    }
}
