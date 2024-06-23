namespace API.Core.Entities;

public class OrderLog : BaseEntity
{
    public int OrderId { get; set; }

    public Order Order { get; set; }

    public int UserId { get; set; }

    public User User { get; set; }

    public DateTime TimestampUTC { get; set; }

    public string Status { get; set; }
}