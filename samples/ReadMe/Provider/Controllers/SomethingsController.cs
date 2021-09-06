using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ReadMe.Provider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SomethingsController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var dataDirectory = Directory.CreateDirectory(Path.Combine("..", "..", "..", "data"));
            var fileData = System.IO.File.ReadAllText(Path.Combine(dataDirectory.FullName, "somethings.json"));
            var somethingsData = string.IsNullOrEmpty(fileData)
                ? new List<Something>()
                : JsonConvert.DeserializeObject<List<Something>>(fileData);
            var requestedSomething = somethingsData.FirstOrDefault(something => string.Equals(something.Id, id, StringComparison.InvariantCultureIgnoreCase));
            if (somethingsData != default)
            {
                return Ok(requestedSomething);
            }
            return BadRequest();
        }
    }
}
