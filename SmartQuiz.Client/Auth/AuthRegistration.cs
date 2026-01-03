using Microsoft.AspNetCore.Components.Authorization;

namespace SmartQuiz.Client.Auth;

public static class AuthRegistration
{
    public static void AddAuthServices(this IServiceCollection services)
    {
        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
    }
}