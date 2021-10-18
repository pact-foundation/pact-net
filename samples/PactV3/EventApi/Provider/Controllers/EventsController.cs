using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provider.Api.Web.Models;

namespace Provider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get([FromQuery] string type)
        {
            IEnumerable<Event> events = _eventRepository.GetAllEvents();

            if (!string.IsNullOrEmpty(type))
            {
                events = events.Where(e => e.EventType.Equals(type, StringComparison.InvariantCultureIgnoreCase));
            }

            return this.Ok(events);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            Event e = _eventRepository.GetAllEvents().First(x => x.EventId == id);
            return this.Ok(e);
        }

        [HttpPost]
        public IActionResult Post(Event @event)
        {
            return @event == null
                       ? this.BadRequest()
                       : this.StatusCode((int)HttpStatusCode.Created);
        }
    }
}
