using System.Collections.Generic;
using Provider.Api.Web.Models;

namespace Provider.Controllers
{
    public interface IEventRepository
    {
        IList<Event> GetAllEvents();
    }
}
