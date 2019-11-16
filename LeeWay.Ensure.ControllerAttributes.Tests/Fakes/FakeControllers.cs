using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// ReSharper disable InconsistentNaming

namespace LeeWay.Ensure.ControllerAttributes.Tests.Fakes
{
    /// <summary>
    /// FakeControllers is a test helper to get MethodInfo objects for the actions under test
    /// </summary>
    public class FakeControllers
    {
        [AllowAnonymous]
        public class Controller_with_AllowAnonymous : Controller
        {
            public IActionResult Action_with_no_attribute()
            {
                return null;
            }
        }

        [Authorize]
        public class Controller_with_authorize_attribute : Controller
        {
            [AllowAnonymous]
            public IActionResult Action_with_AllowAnonymous_attribute()
            {
                return null;
            }

            public IActionResult Action_with_no_attribute()
            {
                return null;
            }
        }

        [Authorize(Policy = PolicyNames.RequireAuthorizedAdmin)]
        public class Controller_with_Policy_RequireAdmin : Controller
        {
            public IActionResult Action_with_no_attribute()
            {
                return null;
            }

            [AllowAnonymous]
            public IActionResult Action_with_AllowAnonymous_attribute()
            {
                return null;
            }

            [Authorize]
            public IActionResult Action_with_Authorize_attribute()
            {
                return null;
            }

            [Authorize(PolicyNames.RequireAuthorizedUser)]
            public IActionResult Action_with_Policy_RequireUser()
            {
                return null;
            }
        }

        public class Controller_with_no_attribute : Controller
        {
            public IActionResult Action_with_no_attribute()
            {
                return null;
            }

            [AllowAnonymous]
            public IActionResult Action_with_AllowAnonymous_attribute()
            {
                return null;
            }

            [Authorize]
            public IActionResult Action_with_Authorize_attribute()
            {
                return null;
            }

            [Authorize(Policy = PolicyNames.RequireAuthorizedAdmin)]
            public IActionResult Action_with_Policy_RequireAdmin()
            {
                return null;
            }
        }
    }
}