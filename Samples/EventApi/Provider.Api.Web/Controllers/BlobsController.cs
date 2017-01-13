using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Provider.Api.Web.Controllers
{
    public class BlobsController : ApiController
    {
        private const string Data = "This is a test";

        [HttpGet]
        [Route("blobs/{id}")]
        public HttpResponseMessage GetById(Guid id)
        {
            var responseContent = new ByteArrayContent(Encoding.UTF8.GetBytes(Data));
            responseContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "text.txt" };
            responseContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = responseContent
            };
        }

        [HttpPost]
        [Route("blobs/{id}")]
        public async Task<HttpResponseMessage> Post(Guid id)
        {
            var bytes = await Request.Content.ReadAsByteArrayAsync();
            var requestBody = Encoding.UTF8.GetString(bytes);

            if (requestBody != Data)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}