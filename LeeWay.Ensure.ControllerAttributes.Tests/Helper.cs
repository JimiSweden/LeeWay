using LeeWay.Ensure.ControllerAttributes.Internal.Rules;
using LeeWay.Ensure.ControllerAttributes.Public;
using LeeWay.Ensure.ControllerAttributes.Tests.Fakes;
using Microsoft.AspNetCore.Authorization;

namespace LeeWay.Ensure.ControllerAttributes.Tests
{
    public static class Helper
    {
        public static IValidationRule ActionRule_withNoAttribute_Controller_hasAllowAnnonymous()
        {
            return new ActionRule<FakeControllers.Controller_with_AllowAnonymous>(
                actionName: nameof(FakeControllers.Controller_with_AllowAnonymous.Action_with_no_attribute),
                attributeRequired: new AuthorizeAttribute(PolicyNames.RequireAuthorizedUser));
        }

        public static IValidationRule ControllerRule_with_policy_RequireAdmin()
        {
            return new ControllerRule<FakeControllers.Controller_with_Policy_RequireAdmin>(
                attributeRequired: new AuthorizeAttribute(PolicyNames.RequireAuthorizedAdmin));
        }
    }
}