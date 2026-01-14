using ActualLab.Fusion.Authentication;

namespace SmartQuiz.Client.Data.Services;

public interface IUserContext : IComputeService
{
    /// <summary>
    /// Get the current authenticated user's ID.
    /// Throws UnauthorizedAccessException if not authenticated.
    /// </summary>
    [ComputeMethod]
    Task<string> GetCurrentUserIdAsync(Session session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the current authenticated user.
    /// Returns null if not authenticated.
    /// </summary>
    [ComputeMethod]
    Task<User?> GetCurrentUserAsync(Session session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the current user is authenticated.
    /// </summary>
    [ComputeMethod]
    Task<bool> IsAuthenticatedAsync(Session session, CancellationToken cancellationToken = default);
}