using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VideoRenderingBackend.Models;
using VideoRenderingBackend.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VideoRenderingBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RenderController : ControllerBase
    {
        // GET: api/<RenderController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<RenderController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value"+id.ToString();
        }

        // POST api/<RenderController>
        [HttpPost]
        public VideoImport Post([FromBody] VideoImport value)
        {
            try
            {
                RenderService service = new RenderService();
                service.GenerateScriptData(value);

                return null;

            }catch(Exception ex)
            {
                return null;
            }
        }

        // PUT api/<RenderController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RenderController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
