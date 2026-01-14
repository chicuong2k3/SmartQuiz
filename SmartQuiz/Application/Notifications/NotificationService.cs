using SmartQuiz.Client.Data.Commands;
using SmartQuiz.Client.Data.Services;

namespace SmartQuiz.Application.Notifications;

public class NotificationService(IServiceProvider services)
    : DbServiceBase<ApplicationDbContext>(services), INotificationService
{
    private readonly IUserContext _userContext = services.GetRequiredService<IUserContext>();

    [ComputeMethod]
    public virtual async Task<IReadOnlyList<NotificationDto>> GetMyNotificationsAsync(
        Session session,
        CancellationToken cancellationToken = default)
    {
        var userId = await _userContext.GetCurrentUserIdAsync(session, cancellationToken);

        await using var dbContext = await DbHub.CreateDbContext(cancellationToken);
        var items = await dbContext.Notifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(30)
            .ToListAsync(cancellationToken);

        // Map server enum -> client enum by name/value, then map to DTO
        return items.Adapt<IReadOnlyList<NotificationDto>>();
    }

    [CommandHandler]
    public virtual async Task MarkReadAsync(MarkNotificationReadCommand command,
        CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
        {
            _ = GetMyNotificationsAsync(command.Session, CancellationToken.None);
            return;
        }

        var userId = await _userContext.GetCurrentUserIdAsync(command.Session, cancellationToken);

        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);
        var entity = await dbContext.Notifications
            .FirstOrDefaultAsync(x => x.Id == command.NotificationId && x.UserId == userId, cancellationToken);

        if (entity == null)
            return;

        if (!entity.IsRead)
        {
            entity.IsRead = true;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    [CommandHandler]
    public virtual async Task MarkAllReadAsync(MarkAllNotificationsReadCommand command,
        CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
        {
            _ = GetMyNotificationsAsync(command.Session, CancellationToken.None);
            return;
        }

        var userId = await _userContext.GetCurrentUserIdAsync(command.Session, cancellationToken);

        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);
        await dbContext.Notifications
            .Where(x => x.UserId == userId && !x.IsRead)
            .ExecuteUpdateAsync(x => x.SetProperty(n => n.IsRead, true), cancellationToken);
    }
}