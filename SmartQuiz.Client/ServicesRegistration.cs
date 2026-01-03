using ActualLab.Fusion.Blazor;
using ActualLab.Fusion.UI;
using MudBlazor.Services;

namespace SmartQuiz.Client;

public static class ServicesRegistration
{
    extension(IServiceCollection services)
    {
        public void RegisterSharedServices()
        {
            services.AddMudServices();
            services.AddScoped<CircuitHub>();
            services.AddScoped<UICommander>();
        }
    }
}