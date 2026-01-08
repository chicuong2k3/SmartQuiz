using System.Security.Claims;
using System.Text.Json;
using ActualLab.Collections;
using ActualLab.Fusion.Authentication;
using ActualLab.Fusion.Authentication.Services;
using ActualLab.Fusion.Internal;
using ActualLab.Time;

namespace SmartQuiz.Application.Auth;

public class LinkedAccountAuthBackendDecorator(
    IAuthBackend inner,
    DbHub<ApplicationDbContext> dbHub,
    IDbUserRepo<ApplicationDbContext, DbUser<string>, string> users,
    IDbSessionInfoRepo<ApplicationDbContext, DbSessionInfo<string>, string> sessions,
    IDbEntityConverter<DbSessionInfo<string>, SessionInfo> sessionConverter,
    IDbUserIdHandler<string> userIdHandler,
    IDbShardResolver<ApplicationDbContext> shardResolver,
    MomentClockSet clockSet,
    IAuth auth)
    : IAuthBackend
{
    [CommandHandler]
    public async Task SignIn(AuthBackend_SignIn command, CancellationToken cancellationToken = default)
    {
        var (session, user, authenticatedIdentity) = (command.Session, command.User, command.AuthenticatedIdentity);
        session.RequireValid();

        var context = CommandContext.GetCurrent();
        var shard = shardResolver.Resolve(command);

        if (Invalidation.IsActive)
        {
            _ = auth.GetSessionInfo(session, CancellationToken.None);
            _ = auth.GetAuthInfo(session, CancellationToken.None);

            var invSessionInfo = context.Operation.Items.KeylessGet<SessionInfo>();
            if (invSessionInfo is not null)
            {
                _ = inner.GetUser(shard, invSessionInfo.UserId, CancellationToken.None);
                _ = auth.GetUserSessions(session, CancellationToken.None);
            }

            return;
        }

        if (!user.Identities.ContainsKey(authenticatedIdentity))
            throw new ArgumentOutOfRangeException(
                $"{nameof(command)}.{nameof(AuthBackend_SignIn.AuthenticatedIdentity)}");

        var dbContext = await dbHub.CreateOperationDbContext(shard, cancellationToken).ConfigureAwait(false);
        await using var _1 = dbContext.ConfigureAwait(false);

        var dbSessionInfo = await sessions.GetOrCreate(dbContext, session.Id, cancellationToken).ConfigureAwait(false);
        var sessionInfo = sessionConverter.ToModel(dbSessionInfo);
        if (sessionInfo.IsSignOutForced)
            throw Errors.SessionUnavailable();

        var isNewUser = false;
        var isOAuthAuth = !authenticatedIdentity.Id.StartsWith("email/");
        var email = user.Claims.GetValueOrDefault(ClaimTypes.Email);

        var dbUser = await users.GetByUserIdentity(
                dbContext,
                authenticatedIdentity,
                forUpdate: true,
                cancellationToken)
            .ConfigureAwait(false);

        if (dbUser is null && isOAuthAuth && !string.IsNullOrEmpty(email))
        {
            var existingUserByEmail = await dbContext.Set<DbUser<string>>()
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
                    updatedClaims = updatedClaims.SetItem(claim.Key, claim.Value);
                }

                existingUserByEmail.Claims = updatedClaims;
                existingUserByEmail.Version++;

                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                dbUser = existingUserByEmail;
            }
        }

        if (dbUser is null)
        {
            (dbUser, isNewUser) = await users.GetOrCreateOnSignIn(
                    dbContext,
                    user,
                    cancellationToken)
                .ConfigureAwait(false);

            if (!isNewUser)
            {
                var updatedClaims = dbUser.Claims;
                foreach (var claim in user.Claims)
                {
                    updatedClaims = updatedClaims.SetItem(claim.Key, claim.Value);
                }

                dbUser.Claims = updatedClaims;
                dbUser.Version++;

                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        else
        {
            var updatedClaims = dbUser.Claims;
            foreach (var claim in user.Claims)
            {
                updatedClaims = updatedClaims.SetItem(claim.Key, claim.Value);
            }

            dbUser.Claims = updatedClaims;
            dbUser.Version++;

            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        sessionInfo = sessionInfo with
        {
            LastSeenAt = clockSet.SystemClock.Now,
            AuthenticatedIdentity = authenticatedIdentity,
            UserId = userIdHandler.Format(dbUser.Id)
        };
        await sessions.Upsert(dbContext, session.Id, sessionInfo, cancellationToken).ConfigureAwait(false);

        context.Operation.Items.KeylessSet(sessionInfo);
        context.Operation.Items.KeylessSet(isNewUser);
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