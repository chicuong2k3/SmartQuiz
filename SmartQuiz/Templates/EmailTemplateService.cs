using System.Reflection;

namespace SmartQuiz.Templates;

public interface IEmailTemplateService
{
    string GetOtpEmailTemplate(string fullName, string otpCode);
}

public class EmailTemplateService : IEmailTemplateService
{
    private readonly Dictionary<string, string> _templateCache = new();
    private readonly Assembly _assembly = typeof(EmailTemplateService).Assembly;

    public string GetOtpEmailTemplate(string fullName, string otpCode)
    {
        var template = LoadTemplate("OtpEmailTemplate.html");

        return template
            .Replace("{{FullName}}", fullName)
            .Replace("{{OtpCode}}", otpCode)
            .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());
    }

    private string LoadTemplate(string templateName)
    {
        if (_templateCache.TryGetValue(templateName, out var cached))
            return cached;

        var resourceName = $"{typeof(EmailTemplateService).Namespace}.{templateName}";

        using var stream = _assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new InvalidOperationException(
                $"Email template '{templateName}' not found. Resource name: {resourceName}");

        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();

        _templateCache[templateName] = template;

        return template;
    }
}