namespace QuickDish.Data.Models;

public class TableSeat : Entity
{
    public Guid TableId { get; set; }
    public string SeatNo { get; set; }  = string.Empty;
}