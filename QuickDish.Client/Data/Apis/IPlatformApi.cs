using QuickDish.Client.Data.Models;
using RestEase;

namespace QuickDish.Client.Data.Apis;

[AllowAnyStatusCode]
public interface IPlatformApi
{
    [Get("api/platforms")]
    Task<IEnumerable<PlatformResponse>> SearchPlatformsAsync([QueryMap] IDictionary<string, object?> query);
}