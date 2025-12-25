namespace QuickDish.Data.Models;

public class Table : Entity
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
}