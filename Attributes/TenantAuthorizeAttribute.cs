using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using BarberShopAPI.Server.Helpers;

namespace BarberShopAPI.Server.Attributes
{
    /// <summary>
    /// Custom authorization attribute that validates JWT token tenant matches request tenant
    /// </summary>
    public class TenantAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Skip authorization if already failed
            if (context.Result != null)
                return;

            // Get tenant from JWT token
            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var tokenTenantIdClaim = user.FindFirst("TenantId")?.Value;
            if (string.IsNullOrEmpty(tokenTenantIdClaim) || !int.TryParse(tokenTenantIdClaim, out var tokenTenantId))
            {
                context.Result = new UnauthorizedObjectResult("Invalid token: Missing tenant information");
                return;
            }

            // Get tenant from current request context
            var requestTenantId = TenantHelper.TryGetCurrentTenantId(context.HttpContext);
            if (requestTenantId == null)
            {
                context.Result = new BadRequestObjectResult("TenantId not found in request context");
                return;
            }

            // Validate tenant matches
            if (tokenTenantId != requestTenantId)
            {
                context.Result = new ForbidResult("Access denied: Token tenant does not match request tenant");
                return;
            }
        }
    }
}
