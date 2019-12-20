using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeeWay.Ensure.ControllerAttributes.Tests.Fakes.Controllers.Duplicate
{
    /*
     * NOTE: This "duplicate" of PublicController is to
     * assert we test that multiple controllers using the same name in the assembly works
     */
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PublicController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}", Name = "Get")]
        public string GetById(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
