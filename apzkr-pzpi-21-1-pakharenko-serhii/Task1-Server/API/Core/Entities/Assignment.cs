namespace API.Core.Entities;

public class Assignment : BaseEntity
{
    public DateTime? DateAssignedUTC { get; set; }
    
    public int UserId { get; set; }
    
    public User User { get; set; }
    
    public int? GuardId { get; set; }
    
    public Guard? Guard { get; set; }
    
    public int Amount { get; set; }
    
    public int Duration { get; set; }
    
    public List<Order> Orders { get; set; }
}