using System.Threading.Tasks;
using Nancy;

namespace PactNet.Mocks.MockHttpService
{
    public interface IMockNancyRequestHandler
    {
        Task<Response> Handle(NancyContext context);
    }
}