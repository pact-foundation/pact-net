using System.Web.Http;

namespace Provider.Api.Web.Controllers
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