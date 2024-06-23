namespace API.Core.Entities;

public class Order : BaseEntity
{
    public int AssignmentId { get; set; }
    
    public Assignment Assignment { get; set; }
    
    public int ThingId { get; set; }
    
    public Thing Thing { get; set; }
    
    public int Frequency { get; set; }
    
    public DateTime? StartTimeUTC { get; set; }
    
    public DateTime? EndTimeUTC { get; set; }
    
    public List<OrderLog> OrderLogs { get; set; }
}