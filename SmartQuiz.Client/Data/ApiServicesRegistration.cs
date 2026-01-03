// using Microsoft.AspNetCore.Components;
// using SmartQuiz.Client.Data.Apis;
// using RestEase;
//
// namespace SmartQuiz.Client.Data;
//
// public static class ApiServicesRegistration
// {
//     public static IServiceCollection RegisterApiServices(this IServiceCollection services)
//     {
//         services.AddScoped(sp =>
//         {
//             var navigationManager = sp.GetRequiredService<NavigationManager>();
//             var baseUri = new Uri(navigationManager.BaseUri);
//             return RestClient.For<IPlatformApi>(baseUri);
//         });
//
//         return services;
//     }
// }

