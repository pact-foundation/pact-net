namespace PactNet.PactVerification
{
    internal interface IMessageInvoker
    {
        string Invoke(PactMessageDescription messageDescription);
    }
}
