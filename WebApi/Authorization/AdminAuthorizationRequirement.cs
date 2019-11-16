using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Authorization
{
    /// <summary>
    /// Handle requirements for admin with role from user claims
    /// </summary>
    public class AdminAuthorizationRequirement : AuthorizationHandler<AdminAuthorizationRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminAuthorizationRequirement requirement)
        {
            //Check user claims for Role Admin
            if (context.User.HasClaim(ClaimTypes.Role, Roles.Admin))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }

    /// <summary>
    /// Handle requirements for user with role from user claims
    /// </summary>
    public class UserAuthorizationRequirement : AuthorizationHandler<UserAuthorizationRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAuthorizationRequirement requirement)
        {
            //todo: inherit / call parent / use ACL type rules?
            if (context.User.HasClaim(ClaimTypes.Role, Roles.User))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}
