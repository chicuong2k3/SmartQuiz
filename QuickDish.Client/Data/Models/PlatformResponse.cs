namespace QuickDish.Client.Data.Models;

public class PlatformResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    
}