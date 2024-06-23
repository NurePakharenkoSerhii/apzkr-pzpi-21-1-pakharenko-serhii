namespace API.Core.Entities;

public class Notification : BaseEntity
{
    public string Message { get; set; }
    
    public bool isRead { get; set; }
    
    public int GuardId { get; set; }
    
    public Guard Guard { get; set; }
}
