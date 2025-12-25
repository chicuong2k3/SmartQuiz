namespace QuickDish.Data.Models;

public class ItemGroup : AuditableEntity
{
    
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}