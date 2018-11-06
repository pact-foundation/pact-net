using System.Web.Http;

namespace PactNet.PactVerification
{
    public interface IProducerHttpProxy
    {
        IHttpActionResult Invoke(PactMessageDescription messageDescription);
    }
}
