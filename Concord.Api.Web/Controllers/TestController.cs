using System.Web.Http;

namespace Concord.Api.Web.Controllers
{
    public class TestController : ApiController
    {
        [Route("test/helloworld")]
        public string Get()
        {
            return "Hello World!";
        }
    }
}