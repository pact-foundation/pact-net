namespace PactNet.PactVerification
{
    public interface IMessageInvoker
    {
        dynamic Invoke(MessagePactDescription description);
    }
}
