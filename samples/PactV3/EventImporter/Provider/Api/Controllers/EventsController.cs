using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Provider.Domain.Handlers;

namespace Provider.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventHandler _eventHandler;

        public EventsController(IEventRepository eventRepository, IEventHandler eventHandler)
        {
            _eventRepository = eventRepository;
            _eventHandler = eventHandler;
        }

        /// <summary>
        /// ImportAll to queue
        /// </summary>
        /// <returns>no content</returns>
        [Authorize]
        [HttpPost]
        public IActionResult ImportToQueue()
        {
            var events = _eventRepository.GetAllEvents();

            _eventHandler.ImportAll(events);

            return NoContent();
        }

        /// <summary>
        /// Dispatch to another queue
        /// </summary>
        /// <returns>no content</returns>
        [Authorize]
        [HttpPost]
        public IActionResult DispatchLastEvent()
        {
            var events = _eventRepository.GetAllEvents();

            _eventHandler.DispatchLast(events);

            return NoContent();
        }
    }
}
