namespace SmartQuiz.Client.Auth;

public static class AuthRequests
{
    public record LoginRequest(
        string Email,
        string Password,
        bool RememberMe);
}