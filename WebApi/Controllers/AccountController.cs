using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : Controller//Base
    {
        // GET: api/Account
        [HttpGet]
        [Authorize("MyPolicyToGetAll")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Account/5
        [HttpGet("{id}")]
        [Authorize("MyPolicyToGetSingle")]
        public string Get(int id)
        {
            return "value";
        }


        /// <summary>
        /// Note: This is the first of two actions named GetWithSimpleRule 
        /// <br/>
        /// This action should get a rule applied
        /// <br/>
        /// Order of the actions inside the controller matters;
        /// first action defined will be first action found by action rule
        /// <br/>
        /// This action is used to validate that only the first method will get the configured rule
        /// when using the simple action rule without parameters
        /// </summary>
        [HttpGet("/simplerule")]
        [Authorize(Policy = "MyPolicyToGetWithSimpleRule")]
        public IReadOnlyList<string> GetWithSimpleRule()
        {
            return new string[] { "value1", "value2" };
        }

        
        /// <summary>
        /// Note: This is the second of two actions named GetWithSimpleRule 
        /// <br/>
        /// This action should NOT get a rule applied
        /// <br/>
        /// Order of the actions inside the controller matters;
        /// first action defined will be first action found by action rule
        /// <br/>
        /// This action is used to validate that only the first method will get the configured rule
        /// when using the simple action rule without parameters
        /// </summary>
        [HttpGet("/simplerule/{id}")]
        public string GetWithSimpleRule(int id)
        {
            return "value";
        }


        // POST: api/Account
        [HttpPost]
        //[Authorize (Policy = "MyPolicyToPost")]
        public void Post([FromBody] string value)
        {
        }


        //POST: api/Account
       [HttpPost]
       [Authorize("MyPolicyToPost")]
        public void Post(CreateSomethingCommand command)
        {
        }

        public class CreateSomethingCommand
        {
            public string NameOrSomething { get; set; }
        }

        // PUT: api/Account/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [Authorize("MyPolicyToDelete")]
        public void Delete(int id)
        {
        }
    }
}
