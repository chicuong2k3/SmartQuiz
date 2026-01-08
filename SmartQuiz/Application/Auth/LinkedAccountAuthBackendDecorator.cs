using System.Security.Claims;
using System.Text.Json;
using ActualLab.Fusion.Authentication;
using ActualLab.Fusion.Authentication.Services;

namespace SmartQuiz.Application.Auth;

public class LinkedAccountAuthBackendDecorator(
    IAuthBackend inner,
    DbHub<ApplicationDbContext> dbHub,
    IDbUserRepo<ApplicationDbContext, DbUser<string>, string> users,
    IDbShardResolver<ApplicationDbContext> shardResolver)
    : IAuthBackend
{
    [CommandHandler]
    public async Task SignIn(AuthBackend_SignIn command, CancellationToken cancellationToken = default)
    {
        var (_, user, authenticatedIdentity) = (command.Session, command.User, command.AuthenticatedIdentity);

        if (!Invalidation.IsActive)
        {
            var shard = shardResolver.Resolve(command);
            var dbContext = await dbHub.CreateOperationDbContext(shard, cancellationToken).ConfigureAwait(false);
            await using var _1 = dbContext.ConfigureAwait(false);

            var isOAuthAuth = !authenticatedIdentity.Id.StartsWith("email/");
            var email = user.Claims.GetValueOrDefault(ClaimTypes.Email);

            if (isOAuthAuth && !string.IsNullOrEmpty(email))
            {
                var dbUser = await users.GetByUserIdentity(
                        (ApplicationDbContext)dbContext,
                        authenticatedIdentity,
                        true,
                        cancellationToken)
                    .ConfigureAwait(false);

                if (dbUser is null)
                {
                    var existingUserByEmail = await ((ApplicationDbContext)dbContext).Set<DbUser<string>>()
                        .Include(u => u.Identities)
                        .Where(u => u.ClaimsJson.Contains(email))
                        .AsAsyncEnumerable()
                        .FirstOrDefaultAsync(u =>
                        {
                            if (string.IsNullOrEmpty(u.ClaimsJson)) return false;
                            var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(u.ClaimsJson);
                            return claims != null &&
                                   claims.TryGetValue(ClaimTypes.Email, out var userEmail) &&
                                   userEmail == email;
                        }, cancellationToken)
                        .ConfigureAwait(false);

                    if (existingUserByEmail != null)
                    {
                        if (existingUserByEmail.Identities.All(i => i.Id != authenticatedIdentity.Id))
                        {
                            existingUserByEmail.Identities.Add(new DbUserIdentity<string>
                            {
                                Id = authenticatedIdentity.Id,
                                DbUserId = existingUserByEmail.Id,
                                Secret = string.Empty
                            });
                        }

                        var updatedClaims = existingUserByEmail.Claims;
                        foreach (var claim in user.Claims)
                        {
                            if (!updatedClaims.ContainsKey(claim.Key))
                                updatedClaims = updatedClaims.SetItem(claim.Key, claim.Value);
                        }

                        existingUserByEmail.Claims = updatedClaims;

                        existingUserByEmail.Version++;

                        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }

        await inner.SignIn(command, cancellationToken).ConfigureAwait(false);
    }

    [CommandHandler]
    public Task<SessionInfo> SetupSession(AuthBackend_SetupSession command,
        CancellationToken cancellationToken = default)
        => inner.SetupSession(command, cancellationToken);

    [CommandHandler]
    public Task SetOptions(AuthBackend_SetSessionOptions command, CancellationToken cancellationToken = default)
        => inner.SetOptions(command, cancellationToken);

    [ComputeMethod(MinCacheDuration = 10)]
    public Task<User?> GetUser(string shard, string userId, CancellationToken cancellationToken = default)
        => inner.GetUser(shard, userId, cancellationToken);
}