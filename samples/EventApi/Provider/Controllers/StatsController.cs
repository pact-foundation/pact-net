using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Provider.Api.Web.Models;

namespace Provider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatsController : ControllerBase
    {
        [HttpGet("status")]
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

        [Route("uptime")]
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
