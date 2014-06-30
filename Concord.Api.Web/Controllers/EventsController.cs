using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Concord.Api.Web.Controllers
{
    public class EventsController : ApiController
    {
        [Route("events")]
        public IEnumerable<dynamic> Get()
        {
            var events = GetAllEventsFromRepo();

            return events;
        }

        private IEnumerable<dynamic> GetAllEventsFromRepo()
        {
            return new List<dynamic>
            {
                new
                {
                    EventId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                    Timestamp = "2014-06-30T01:37:41.0660548Z",
                    EventType = "JobSearchView"
                },
                new
                {
                    EventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                    Timestamp = "2014-06-30T01:37:52.2618864Z",
                    EventType = "JobDetailsView"
                },
                new
                {
                    EventId = Guid.Parse("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
                    Timestamp = "2014-06-30T01:38:00.8518952Z",
                    EventType = "JobSearchView"
                }
            };
        }
    }
}