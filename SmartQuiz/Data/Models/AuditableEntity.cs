namespace SmartQuiz.Data.Models;

public abstract class AuditableEntity : Entity
{
    public string CreatedBy { get; set; } = string.Empty;
}