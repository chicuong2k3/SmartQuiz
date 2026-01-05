using System.Data;
using ActualLab.Fusion.Authentication;
using ActualLab.Fusion.Authentication.Services;
using ActualLab.Fusion.Blazor;
using ActualLab.Fusion.Blazor.Authentication;
using ActualLab.Fusion.EntityFramework.Operations;
using ActualLab.Fusion.EntityFramework.Operations.LogProcessing;
using ActualLab.Fusion.Server;
using ActualLab.Fusion.Server.Authentication;
using ActualLab.Fusion.Server.Endpoints;
using ActualLab.Fusion.Server.Middlewares;
using ActualLab.Rpc;
using Microsoft.AspNetCore.Authentication.Cookies;
using SmartQuiz.Application;
using SmartQuiz.Client.Data.Services;

namespace SmartQuiz;

public static class ServicesExtensions
{
    public static IServiceCollection RegisterFusionServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMapster();

#if !DEBUG
        Interceptor.Options.Defaults.IsValidationEnabled = false;
#endif

        DbOperationScope.Options.DefaultIsolationLevel = IsolationLevel.RepeatableRead;
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddPooledDbContextFactory<ApplicationDbContext>(db =>
        {
            db.UseNpgsql(connectionString);
            //db.UseNpgsqlHintFormatter();
            db.EnableSensitiveDataLogging();
        });

        services.AddDbContextServices<ApplicationDbContext>(db =>
        {
            db.AddOperations(operations =>
            {
                operations.ConfigureOperationLogReader(_ => new DbOperationLogReader<ApplicationDbContext>.Options
                {
                    CheckPeriod = TimeSpan.FromSeconds(5),
                });
                operations.ConfigureEventLogReader(_ => new DbEventLogReader<ApplicationDbContext>.Options
                {
                    CheckPeriod = TimeSpan.FromSeconds(5),
                });
                operations.AddNpgsqlOperationLogWatcher();
            });
        });

        services.AddFusion(RpcServiceMode.Server, true, fusion =>
        {
            fusion.AddWebServer(true)
                .ConfigureSessionMiddleware(_ => new SessionMiddleware.Options())
                .ConfigureAuthEndpoint(_ => new AuthEndpoints.Options
                {
                    SignInPropertiesBuilder = (_, properties) => { properties.IsPersistent = true; }
                })
                .ConfigureServerAuthHelper(_ => new ServerAuthHelper.Options
                {
                    NameClaimKeys = [],
                });

            fusion.AddDbAuthService<ApplicationDbContext, DbSessionInfo<string>, DbUser<string>, string>(auth =>
            {
                auth.ConfigureSessionInfoEntityResolver(_ =>
                    new DbEntityResolver<ApplicationDbContext, string, DbSessionInfo<string>>.Options
                    {
                        KeyExtractor = s => s.Id
                    });

                auth.ConfigureUserEntityResolver(_ =>
                    new DbEntityResolver<ApplicationDbContext, string, DbUser<string>>.Options
                    {
                        KeyExtractor = u => u.Id
                    }, includeIdentities: true);
            });

            fusion.AddBlazor()
                .AddAuthentication()
                .AddPresenceReporter();

            fusion.AddOperationReprocessor();


            fusion.AddServer<ICustomAuthService, CustomAuthService>();

            fusion.AddServer<IFlashcardService, FlashcardService>();
            fusion.AddServer<IFlashcardSetService, FlashcardSetService>();
            fusion.AddServer<IQuizResultService, QuizResultService>();
        });

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = "/signIn";
                options.LogoutPath = "/signOut";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                options.Events.OnSigningIn = ctx =>
                {
                    ctx.CookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(28);
                    return Task.CompletedTask;
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
                options.CallbackPath = "/authentication/signin-google";
                options.SaveTokens = true;
            })
            .AddFacebook(options =>
            {
                options.AppId = configuration["Authentication:Facebook:AppId"]!;
                options.AppSecret = configuration["Authentication:Facebook:AppSecret"]!;
                options.CallbackPath = "/authentication/signin-facebook";
                options.SaveTokens = true;
            });

        return services;
    }
}