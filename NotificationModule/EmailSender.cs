using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace NotificationModule;

internal class EmailSender(IOptions<EmailSettings> settings, ILogger<EmailSender> logger) : IEmailSender
{
    private readonly EmailSettings _settings = settings.Value;

    public Task SendEmailAsync(string to, string? subject, string body, bool isHtml = true)
    {
        return SendEmailAsync(new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml
        });
    }

    public async Task SendEmailAsync(EmailMessage message)
    {
        var email = new MimeMessage();

        // From
        var fromAddress = message.From ?? _settings.SenderEmail;
        email.From.Add(new MailboxAddress(_settings.SenderName, fromAddress));

        // To
        email.To.Add(MailboxAddress.Parse(message.To));

        // CC
        foreach (var cc in message.Cc) email.Cc.Add(MailboxAddress.Parse(cc));

        // BCC
        foreach (var bcc in message.Bcc) email.Bcc.Add(MailboxAddress.Parse(bcc));

        // Subject
        email.Subject = message.Subject;

        // Body
        var builder = new BodyBuilder();
        if (message.IsHtml)
            builder.HtmlBody = message.Body;
        else
            builder.TextBody = message.Body;

        // Attachments
        foreach (var attachment in message.Attachments)
            builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));

        email.Body = builder.ToMessageBody();

        // Send
        using var smtp = new SmtpClient();

        var secureSocketOptions = _settings.UseSsl
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.None;

        await smtp.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, secureSocketOptions);
        if (!string.IsNullOrWhiteSpace(_settings.Username))
        {
            await smtp.AuthenticateAsync(
                _settings.Username,
                _settings.Password
            );
        }

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        logger.LogInformation("Email sent to {To} with subject {Subject}", message.To, message.Subject);
    }
}