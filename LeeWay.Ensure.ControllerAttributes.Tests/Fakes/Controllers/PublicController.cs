using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeeWay.Ensure.ControllerAttributes.Tests.Fakes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PublicController : ControllerBase
    {
        // GET: api/Public
        [HttpGet]
        [Authorize("MyPolicy")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Public/5

        [HttpGet("{id}", Name = "Get")]
        [Authorize("MyPolicy")]
        public string GetById(int id)
        {
            return "value";
        }

        // POST: api/Public
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Public/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [Authorize("DeletePolicy")]
        public void Delete(int id)
        {
        }

    }
}
