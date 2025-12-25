using MudBlazor.Services;
using QuickDish.Client.Data;

namespace QuickDish.Client;

public static class ServicesRegistration
{
    extension(IServiceCollection services)
    {
        public void RegisterServices()
        {
            services.AddMudServices();

            services.RegisterApiServices();
        }
    }
}