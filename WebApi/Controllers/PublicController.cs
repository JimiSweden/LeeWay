using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
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

        [HttpGet("{id}")]
        [AuthorizeAttribute("MyPolicy")]
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
