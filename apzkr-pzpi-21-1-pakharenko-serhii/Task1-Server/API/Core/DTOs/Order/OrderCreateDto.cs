namespace API.Core.DTOs.Order;

public record OrderCreateDto
{
    public int ThingId { get; set; }
    public int AssignmentId { get; set; }
    public int Frequency { get; set; }
    public DateTime StartTimeUTC { get; set; }
    public DateTime EndTimeUTC { get; set; }
}