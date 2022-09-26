namespace Ordering;

public class Order
{
    public int Id { get; set; }
    
    public DateTime DateCreated { get; set; }
    
    public Guid ExternalId { get; set; }
    
    public string? Description { get; set; }
}