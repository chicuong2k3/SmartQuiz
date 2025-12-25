namespace QuickDish.Data.Models;

public class Order : AuditableEntity
{
    public string Title { get; set; }
    public string? Description { get; set; }

    public Guid? TableId { get; set; }
    public Guid? TableSeatId { get; set; }
    
    public OrderStatus Status { get; set; } = default!;
    public string CashierId { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; } = default!;
    public DateTime? PaidAt { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderType OrderType { get; set; } = default!;

    public OrderChannel? OrderChannel { get; set; }
}

public enum OrderStatus
{
    Open,
    Paid,
    Cancelled
}

public enum PaymentMethod
{
    Ewallet,
    Cash
}

public enum OrderType 
{
    DineIn,
    TakeAway
}

public enum OrderChannel 
{
    Customer,
    Staff
}


