using System.Web.Http;

namespace Provider.Api.Web.Controllers
{
    public class VersionController : ApiController
    {
        [Route("version")]
        public dynamic GetVersion()
        {
            return "1.0.22";
        }
    }
}