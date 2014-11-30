using System.Web.Http;

namespace Provider.Api.Web.Controllers
{
    public class StatsController : ApiController
    {
        [Route("stats/status")]
        public dynamic GetAlive()
        {
            return new { alive = true };
        }
    }
}