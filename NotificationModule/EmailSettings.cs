namespace NotificationModule;

public class EmailSettings
{
    public const string SectionName = nameof(EmailSettings);

    public required string SmtpServer { get; init; }
    public int SmtpPort { get; init; } = 587;
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string SenderEmail { get; init; }
    public string SenderName { get; init; } = string.Empty;
    public bool UseSsl { get; init; } = true;
}