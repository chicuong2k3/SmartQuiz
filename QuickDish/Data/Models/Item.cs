namespace QuickDish.Data.Models;
 
public class Item : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public bool IsAvailable { get; set; }
    public Guid ItemGroupId { get; set; }
}