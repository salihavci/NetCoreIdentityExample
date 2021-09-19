using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample.Helpers.RequirementProvider
{
    public class ExpireDateExchangeRequirement:IAuthorizationRequirement
    {

    }

    public class ExpireDateExchangeHandler : AuthorizationHandler<ExpireDateExchangeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExpireDateExchangeRequirement requirement)
        {
            if (context.User.Identity != null && context.User != null)
            {
                var claims = context.User.Claims.Where(m=> m.Type == "ExpireDateExchange" && m.Value != null).FirstOrDefault();
                if (claims != null)
                {
                    if (DateTime.Now < Convert.ToDateTime(claims.Value))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
