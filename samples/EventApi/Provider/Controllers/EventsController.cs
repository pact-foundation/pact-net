using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Provider.Api.Web.Models;

namespace Provider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult Get([FromQuery] string type)
        {
            IEnumerable<Event> events = this.GetAllEventsFromRepo();

            if (!string.IsNullOrEmpty(type))
            {
                events = events.Where(e => e.EventType.Equals(type, StringComparison.InvariantCultureIgnoreCase));
            }

            return this.Ok(events);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            Event e = this.GetAllEventsFromRepo().First(x => x.EventId == id);
            return this.Ok(e);
        }

        [HttpPost]
        public IActionResult Post(Event @event)
        {
            return @event == null
                       ? this.BadRequest()
                       : this.StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPost]
        [Route("upload-file")]
        [Consumes("multipart/form-data")]
        public IActionResult FileUpload()
        {
            var singleFile = Request.Form.Files.SingleOrDefault(f => f.Name == "file");
            if (singleFile == null || Request.Form.Files.Count != 1)
            {
                return BadRequest("Request must contain a single file with a parameter named 'file'");
            }
            if (singleFile.ContentType != "image/jpeg")
            {
                return BadRequest("File content-type must be image/jpeg");
            }
            return StatusCode(201);
        }

        private IEnumerable<Event> GetAllEventsFromRepo()
        {
            return new List<Event>
            {
                new Event
                {
                    EventId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                    Timestamp = DateTime.Parse("2014-06-30T01:37:41.0660548"),
                    EventType = "SearchView"
                },
                new Event
                {
                    EventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                    Timestamp = DateTime.Parse("2014-06-30T01:37:52.2618864"),
                    EventType = "DetailsView"
                },
                new Event
                {
                    EventId = Guid.Parse("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
                    Timestamp = DateTime.Parse("2014-06-30T01:38:00.8518952"),
                    EventType = "SearchView"
                }
            };
        }
    }
}
