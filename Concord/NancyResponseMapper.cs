using System.Text;
using Nancy;
using Newtonsoft.Json;

namespace Concord
{
    public class NancyResponseMapper
    {
        public Response Convert(PactServiceResponse from)
        {
            if (from == null)
                return null;

            var to = new Response
                         {
                             StatusCode = (HttpStatusCode) from.Status, 
                             Headers = from.Headers
                         };

            if (from.Body != null)
            {
                string jsonBody = JsonConvert.SerializeObject(from.Body);
                to.Contents = s =>
                                  {
                                      byte[] bytes = Encoding.UTF8.GetBytes(jsonBody);
                                      s.Write(bytes, 0, bytes.Length);
                                      s.Flush();
                                  };
            }

            return to;
        }
    }
}