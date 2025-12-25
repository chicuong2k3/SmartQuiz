namespace QuickDish.Client.Auth;

public static class UserProfileRequests
{
    public record RegisterUserRequest(
        string Email,
        string Password);

    public record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword);
}