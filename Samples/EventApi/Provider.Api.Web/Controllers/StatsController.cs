using System.Web.Http;

namespace Provider.Api.Web.Controllers
{
    public class StatsController : ApiController
    {
        [Route("stats/status")]
        public string GetAlive()
        {
            return "alive";
        }
    }
}