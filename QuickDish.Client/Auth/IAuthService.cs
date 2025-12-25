using RestEase;

namespace QuickDish.Client.Auth;

[AllowAnyStatusCode]
public interface IAuthService
{
    [Get("authentication/login")]
    Task LoginAsync(AuthRequests.LoginRequest request);
}