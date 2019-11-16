using Microsoft.AspNetCore.Mvc;

namespace LeeWay.Ensure.ControllerAttributes.Tests.Fakes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizedController : ControllerBase
    {
    }
}