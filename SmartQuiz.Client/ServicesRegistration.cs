using ActualLab.Fusion.Blazor;
using ActualLab.Fusion.UI;
using MudBlazor.Services;
using MudExtensions.Services;

namespace SmartQuiz.Client;

public static class ServicesRegistration
{
    extension(IServiceCollection services)
    {
        public void RegisterSharedServices()
        {
            services.AddMudServices();
            services.AddMudExtensions();
            services.AddScoped<CircuitHub>();
            services.AddScoped<UICommander>();
        }
    }
}