using System.Collections.Generic;
using Provider.Api.Web.Models;

namespace Provider.Controllers
{
    public class EmptyEventRepository : IEventRepository
    {
        public IList<Event> GetAllEvents()
        {
            //On this app, the exercise is to add the events on the provider tests through provider states
            return new List<Event>();
        }
    }
}
