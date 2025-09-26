using Microsoft.AspNetCore.Http;

namespace BarberShopAPI.Server.Helpers
{
    public static class TenantHelper
    {
        /// <summary>
        /// Extracts the current tenant ID from the HTTP context.
        /// This should be called after TenantMiddleware has processed the request.
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <returns>The tenant ID if found, otherwise throws an exception</returns>
        /// <exception cref="BadHttpRequestException">Thrown when TenantId is not found in context</exception>
        public static int GetCurrentTenantId(HttpContext context)
        {
            if (!context.Items.ContainsKey("TenantId"))
            {
                throw new BadHttpRequestException("TenantId not found in context. Ensure TenantMiddleware is properly configured.");
            }
            
            return (int)context.Items["TenantId"]!;
        }

        /// <summary>
        /// Safely extracts the current tenant ID from the HTTP context.
        /// Returns null if not found instead of throwing an exception.
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <returns>The tenant ID if found, otherwise null</returns>
        public static int? TryGetCurrentTenantId(HttpContext context)
        {
            if (!context.Items.ContainsKey("TenantId"))
            {
                return null;
            }
            
            return (int)context.Items["TenantId"];
        }

        /// <summary>
        /// Checks if the current request has a valid tenant context.
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <returns>True if tenant context exists, otherwise false</returns>
        public static bool HasTenantContext(HttpContext context)
        {
            return context.Items.ContainsKey("TenantId");
        }

        /// <summary>
        /// Extracts the current tenant name from the HTTP context.
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <returns>The tenant name if found, otherwise null</returns>
        public static string? GetCurrentTenantName(HttpContext context)
        {
            if (!context.Items.ContainsKey("TenantName"))
            {
                return null;
            }
            
            return (string)context.Items["TenantName"];
        }
    }
}
