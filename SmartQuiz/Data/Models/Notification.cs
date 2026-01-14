namespace SmartQuiz.Data.Models;

public class Notification : Entity
{
    private Notification()
    {
    }

    public Notification(string userId, string title, string? message, NotificationType type)
    {
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        IsRead = false;
    }

    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
}