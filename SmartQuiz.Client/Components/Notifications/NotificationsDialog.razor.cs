using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SmartQuiz.Client.Components.Notifications;

public partial class NotificationsDialog
{
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] public List<NotificationItem> Items { get; set; } = new();

    private void Close() => MudDialog.Close();

    private void MarkAllRead()
    {
        foreach (var i in Items)
            i.IsRead = true;
    }

    private void OnItemClick(NotificationItem item)
    {
        item.IsRead = true;
        // TODO: navigate to item.Url
    }

    private static string GetIcon(NotificationType type) => type switch
    {
        NotificationType.Info => Icons.Material.Filled.Info,
        NotificationType.Success => Icons.Material.Filled.CheckCircle,
        NotificationType.Warning => Icons.Material.Filled.Warning,
        NotificationType.Error => Icons.Material.Filled.Error,
        _ => Icons.Material.Filled.Notifications,
    };

    private static Color GetAvatarColor(NotificationType type) => type switch
    {
        NotificationType.Success => Color.Success,
        NotificationType.Warning => Color.Warning,
        NotificationType.Error => Color.Error,
        _ => Color.Info,
    };

    public class NotificationItem
    {
        public string Title { get; set; } = "";
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; }
        public NotificationType Type { get; set; } = NotificationType.Info;
    }

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error,
    }
}