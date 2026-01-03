using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartQuiz.Client.Auth;

namespace SmartQuiz.Auth;

internal static class LoginLogoutEndpointRouteBuilderExtensions
{
    extension(IEndpointRouteBuilder endpoints)
    {
        internal IEndpointConventionBuilder MapLoginAndLogout()
        {
            var group = endpoints.MapGroup("authentication");

            var idps = new[] { "Google", "Facebook" };
            foreach (var provider in idps)
            {
                group.MapGet($"/signin-{provider.ToLower()}", (HttpContext _, string? returnUrl) =>
                {
                    var props = GetAuthProperties(returnUrl);
                    return Results.Challenge(props, [provider]);
                });
            }

            group.MapPost("/signin", async (HttpContext context, [FromBody] AuthRequests.LoginRequest request) =>
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return Results.BadRequest("Email and password are required.");
                }

                var signInManager = context.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
                var result = await signInManager.PasswordSignInAsync(request.Email, request.Password,
                    isPersistent: request.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var claimsPrincipal = context.User;
                    await context.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);
                    return Results.Ok("Sign-in successful.");
                }
                else
                {
                    return Results.Unauthorized();
                }
            });


            group.MapGet("/logout", async (HttpContext context, [FromQuery] string? returnUrl) =>
            {
                var props = GetAuthProperties(returnUrl);

                var oidcOptionsMonitor = context.RequestServices.GetRequiredService<
                    IOptionsMonitor<OpenIdConnectOptions>>();
                var oidcOptions = oidcOptionsMonitor.Get(OpenIdConnectDefaults.AuthenticationScheme);

                if (oidcOptions.Configuration?.EndSessionEndpoint != null)
                {
                    await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, props);
                }

                await context.SignOutAsync(IdentityConstants.ApplicationScheme, props);
            });


            return group;
        }
    }

    private static AuthenticationProperties GetAuthProperties(string? returnUrl)
    {
        // TODO: Use HttpContext.Request.PathBase instead.
        const string PATH_BASE = "/";

        // Prevent open redirects.
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = PATH_BASE;
        }
        else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
        {
            returnUrl = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
        }
        else if (returnUrl[0] != '/')
        {
            returnUrl = $"{PATH_BASE}{returnUrl}";
        }

        return new AuthenticationProperties { RedirectUri = returnUrl };
    }
}