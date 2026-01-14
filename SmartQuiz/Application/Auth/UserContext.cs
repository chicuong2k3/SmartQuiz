using ActualLab.Fusion.Authentication;
using SmartQuiz.Client.Data.Services;

namespace SmartQuiz.Application.Auth;

public class UserContext(IAuth auth) : IUserContext
{
    public virtual async Task<string> GetCurrentUserIdAsync(
        Session session,
        CancellationToken cancellationToken = default)
    {
        var user = await auth.GetUser(session, cancellationToken);

        if (user == null || !user.IsAuthenticated())
            throw new UnauthorizedAccessException("User must be authenticated");

        return user.Id;
    }

    public virtual async Task<User?> GetCurrentUserAsync(
        Session session,
        CancellationToken cancellationToken = default)
    {
        return await auth.GetUser(session, cancellationToken);
    }

    public virtual async Task<bool> IsAuthenticatedAsync(
        Session session,
        CancellationToken cancellationToken = default)
    {
        var user = await auth.GetUser(session, cancellationToken);
        return user?.IsAuthenticated() ?? false;
    }
}