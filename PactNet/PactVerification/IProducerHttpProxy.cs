namespace PactNet.PactVerification
{
    public interface IProducerHttpProxy
    {
        string Invoke(dynamic messageDescription);
    }
}
