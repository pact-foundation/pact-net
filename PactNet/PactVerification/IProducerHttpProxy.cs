namespace PactNet.PactVerification
{
    public interface IProducerHttpProxy
    {
        string Invoke(PactMessageDescription messageDescription);
    }
}
