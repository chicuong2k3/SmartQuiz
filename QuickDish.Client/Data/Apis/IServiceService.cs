using QuickDish.Client.Data.Models;
using QuickDish.Client.Data.Requests;

namespace QuickDish.Client.Data.Apis;

public interface IServiceService
{
    Task<IEnumerable<ServiceResponse>> SearchServicesAsync(SearchServicesRequest request);
}

internal class ServiceService : IServiceService
{

    public ServiceService()
    {
    }

    public async Task<IEnumerable<ServiceResponse>> SearchServicesAsync(SearchServicesRequest request)
    {
        // Simulate an API call with a delay
        await Task.Delay(500);

        // Return mock data for demonstration purposes
        return new List<ServiceResponse>
        {
            new ServiceResponse
            {
                Name = "Instagram Followers",
                Description = "Get real Instagram followers quickly.",
                Price = 9.99m
            },
            new ServiceResponse
            {
                Name = "Twitter Likes",
                Description = "Boost your tweets with genuine likes.",
                Price = 4.99m
            }
        };
    }
}