using System;
using System.Collections.Generic;
using System.Web.Http;
using Provider.Api.Web.Models;

namespace Provider.Api.Web.Controllers
{
    public class StatsController : ApiController
    {
        [Route("stats/status")]
        public dynamic GetStatus()
        {
            return new
            {
                alive = true,
                _links = new Dictionary<string, HypermediaLink>
                {
                    { "self", new HypermediaLink("/stats/status") },
                    { "uptime", new HypermediaLink("/stats/uptime") },
                    { "version", new HypermediaLink("/stats/version") }
                }
            };
        }

        [Route("stats/uptime")]
        public dynamic GetUptime()
        {
            return new
            {
                upSince = new DateTime(2014, 6, 27, 23, 51, 12, DateTimeKind.Utc),
                _links = new Dictionary<string, HypermediaLink>
                {
                    { "self", new HypermediaLink("/stats/uptime") }
                }
            };
        }
    }
}