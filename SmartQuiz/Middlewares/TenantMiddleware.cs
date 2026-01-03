namespace SmartQuiz.Middlewares;

public class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Extract tenant from subdomain
        var tenant = context.Request.Host.Host.Split('.')[0];

        context.Items["Tenant"] = tenant;

        await next(context);
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<TenantMiddleware>();
}