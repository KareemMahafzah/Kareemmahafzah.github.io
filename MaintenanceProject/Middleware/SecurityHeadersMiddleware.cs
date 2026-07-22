using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MaintenanceProject.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _csp;

        public SecurityHeadersMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            // Read CSP from configuration, fallback to a safe default
            _csp = configuration["SecurityHeaders:ContentSecurityPolicy"] ?? "default-src 'self'";
        }

        public async Task Invoke(HttpContext context)
        {
            var headers = context.Response.Headers;
            if (!headers.ContainsKey("X-Content-Type-Options")) headers["X-Content-Type-Options"] = "nosniff";
            if (!headers.ContainsKey("X-Frame-Options")) headers["X-Frame-Options"] = "DENY";
            if (!headers.ContainsKey("Referrer-Policy")) headers["Referrer-Policy"] = "no-referrer";
            if (!headers.ContainsKey("Permissions-Policy")) headers["Permissions-Policy"] = "geolocation=()";
            if (!headers.ContainsKey("Content-Security-Policy")) headers["Content-Security-Policy"] = _csp;

            await _next(context);
        }
    }

    public static class SecurityHeadersExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
