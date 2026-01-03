using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SmartQuiz.Auth;

public static class AuthRegistration
{
    public static void AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
        services.AddCascadingAuthenticationState();
        services.AddAuthorization();

        var oidcSection = configuration.GetSection("Oidc");

        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddOpenIdConnect("Google", options =>
            {
                oidcSection.GetSection("Google").Bind(options);
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.MapInboundClaims = false;

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                options.ClaimActions.Clear();
                options.ClaimActions.MapJsonKey("sub", "sub");
                options.ClaimActions.MapJsonKey("email", "email");
                options.ClaimActions.MapJsonKey("picture", "picture");

                options.Events.OnTicketReceived = async context =>
                {
                    if (context.Principal == null)
                        return;

                    var userManager = context.HttpContext.RequestServices
                        .GetRequiredService<UserManager<ApplicationUser>>();

                    var user = await MapUserToInternalAccount(userManager, context.Principal, "Google");
                    if (user is null)
                        return;

                    var signInManager = context.HttpContext.RequestServices
                        .GetRequiredService<SignInManager<ApplicationUser>>();
                    await signInManager.SignInAsync(user, isPersistent: false);
                };
            })
            .AddFacebook(options =>
            {
                oidcSection.GetSection("Facebook").Bind(options);
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.SaveTokens = true;

                options.Scope.Add("public_profile");
                options.Scope.Add("email");

                options.Fields.Add("email");
                options.Fields.Add("picture");

                options.ClaimActions.Clear();
                options.ClaimActions.MapJsonKey("sub", "id");
                options.ClaimActions.MapJsonKey("email", "email");
                options.ClaimActions.MapJsonKey("picture", "picture.data.url");

                options.Events.OnCreatingTicket = async context =>
                {
                    if (context.Principal == null)
                        return;

                    foreach (var c in context.Principal.Claims)
                        Console.WriteLine($"{c.Type} = {c.Value}");

                    var userManager = context.HttpContext.RequestServices
                        .GetRequiredService<UserManager<ApplicationUser>>();

                    var user = await MapUserToInternalAccount(userManager, context.Principal, "Facebook");
                    if (user is null)
                        return;

                    var signInManager = context.HttpContext.RequestServices
                        .GetRequiredService<SignInManager<ApplicationUser>>();
                    await signInManager.SignInAsync(user, isPersistent: false);
                };
            });

        services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();


        services.ConfigureCookieOidcRefresh(
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();


        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
    }

    private static async Task<ApplicationUser?> MapUserToInternalAccount(
        UserManager<ApplicationUser> userManager,
        ClaimsPrincipal principal,
        string provider)
    {
        var providerKey = principal.FindFirstValue("sub");
        var email = principal.FindFirstValue("email");
        var picture = principal.FindFirstValue("picture");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(providerKey))
        {
            return null;
        }

        var user = await userManager.Users
            .Include(u => u.UserLogins)
            .FirstOrDefaultAsync(u =>
                u.UserLogins.Any(l => l.LoginProvider == provider && l.ProviderKey == providerKey));

        if (user != null)
            return user;

        user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            await userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerKey, provider));
            return user;
        }

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            Picture = picture
        };

        await userManager.CreateAsync(user);
        await userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerKey, provider));
        return user;
    }
}