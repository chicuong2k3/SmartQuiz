namespace SmartQuiz.Client.Data.Services;

public interface INotificationService : IComputeService
{
    [ComputeMethod]
    Task<IReadOnlyList<NotificationDto>> GetMyNotificationsAsync(Session session,
        CancellationToken cancellationToken = default);

    [CommandHandler]
    Task MarkReadAsync(MarkNotificationReadCommand command, CancellationToken cancellationToken = default);

    [CommandHandler]
    Task MarkAllReadAsync(MarkAllNotificationsReadCommand command, CancellationToken cancellationToken = default);
}