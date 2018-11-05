namespace PactNet.PactVerification
{
    public interface IProducerHttpProxy
    {
        string Invoke(PactMessageDescription description);
    }
}
