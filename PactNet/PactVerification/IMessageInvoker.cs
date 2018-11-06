namespace PactNet.PactVerification
{
    public interface IMessageInvoker
    {
        object Invoke(PactMessageDescription messageDescription);
    }
}
