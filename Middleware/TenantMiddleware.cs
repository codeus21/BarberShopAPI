// Add this to your C# backend project
using Microsoft.EntityFrameworkCore;
using BarberShopAPI.Server.Data;
using BarberShopAPI.Server.Models;

namespace BarberShopAPI.Server.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantMiddleware> _logger;

        public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, BarberShopContext dbContext)
        {
            try
            {
                // Skip tenant resolution for non-tenant endpoints
                var path = context.Request.Path.Value?.ToLower();
                if (ShouldSkipTenantResolution(path))
                {
                    await _next(context);
                    return;
                }

                var tenant = await ExtractTenantAsync(context, dbContext);
                
                if (tenant == null)
                {
                    _logger.LogWarning("Tenant not found for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Barber shop not found");
                    return;
                }

                if (!tenant.IsActive)
                {
                    _logger.LogWarning("Inactive tenant accessed: {TenantId}", tenant.Id);
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Barber shop is not active");
                    return;
                }

                // Store tenant information in context
                context.Items["TenantId"] = tenant.Id;
                context.Items["Tenant"] = tenant;
                context.Items["TenantSubdomain"] = tenant.Subdomain;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in tenant middleware");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal server error");
                return;
            }

            await _next(context);
        }

        private async Task<BarberShop?> ExtractTenantAsync(HttpContext context, BarberShopContext dbContext)
        {
            // Method 1: Check for tenant parameter in URL (for testing)
            var query = context.Request.Query;
            if (query.ContainsKey("tenant"))
            {
                var tenantParam = query["tenant"].ToString().ToLower();
                _logger.LogInformation("Using tenant parameter: {Tenant}", tenantParam);
                return await dbContext.BarberShops
                    .FirstOrDefaultAsync(b => b.Subdomain == tenantParam);
            }

            // Method 2: Extract from subdomain (e.g., elite.thebarberbook.com)
            var host = context.Request.Host.Host.ToLower();
            var subdomain = ExtractSubdomain(host);
            
            if (!string.IsNullOrEmpty(subdomain))
            {
                _logger.LogInformation("Using subdomain: {Subdomain}", subdomain);
                return await dbContext.BarberShops
                    .FirstOrDefaultAsync(b => b.Subdomain == subdomain);
            }

            // Method 3: Extract from URL path (e.g., /elite/booker)
            var pathSegments = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (pathSegments?.Length > 0)
            {
                var firstSegment = pathSegments[0];
                if (firstSegment != "api" && firstSegment != "admin")
                {
                    _logger.LogInformation("Using path segment: {PathSegment}", firstSegment);
                    return await dbContext.BarberShops
                        .FirstOrDefaultAsync(b => b.Subdomain == firstSegment);
                }
            }

            // Method 4: Default tenant for development and direct API access
            if (host.Contains("localhost") || 
                host.Contains("railway.app") ||
                host.Contains("vercel.app") ||
                host.Contains("railway"))
            {
                _logger.LogInformation("Using default tenant for development");
                return await dbContext.BarberShops
                    .FirstOrDefaultAsync(b => b.Subdomain == "default");
            }

            return null;
        }

        private bool ShouldSkipTenantResolution(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            // Skip tenant resolution for these paths
            var skipPaths = new[]
            {
                "/health",
                "/api/health",
                "/swagger",
                "/swagger/",
                "/swagger/index.html",
                "/swagger/v1/swagger.json",
                "/favicon.ico",
                "/",
                "/api/tenant" // Allow tenant management endpoints
            };

            // Check exact matches
            if (skipPaths.Contains(path))
            {
                return true;
            }

            // Check if path starts with any skip pattern
            if (path.StartsWith("/health") || 
                path.StartsWith("/swagger") || 
                path.StartsWith("/api/tenant"))
            {
                return true;
            }

            return false;
        }

        private string? ExtractSubdomain(string host)
        {
            // Skip Railway and other hosting platform hostnames for direct API access
            if (host.Contains("railway.app") || 
                host.Contains("vercel.app") || 
                host.Contains("localhost") ||
                host.Contains("127.0.0.1"))
            {
                return null;
            }

            // Extract subdomain from hostname
            // Examples: 
            // - elite.thebarberbook.com -> elite
            // - default.thebarberbook.com -> default
            // - barbershop1.example.com -> barbershop1
            var parts = host.Split('.');
            if (parts.Length >= 3)
            {
                var subdomain = parts[0];
                // Skip common subdomains that aren't tenant names
                var skipSubdomains = new[] { "www", "api", "admin", "app", "www2", "mail" };
                if (!skipSubdomains.Contains(subdomain.ToLower()))
                {
                    return subdomain.ToLower();
                }
            }
            return null;
        }
    }

    // Extension method to register the middleware
    public static class TenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}
