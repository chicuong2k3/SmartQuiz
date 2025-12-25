namespace QuickDish.Data.Models;

public class OrderItem : AuditableEntity
{
    public Guid OrderId { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }

}