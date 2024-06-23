using API.Core.DTOs.Thing;

namespace API.Core.DTOs.Order;

public record AssignmentOrderDto
{
    public ThingDto Thing { get; set; }
    public int Frequency { get; set; }
    public DateTime? StartTimeUTC { get; set; }
    public DateTime? EndTimeUTC { get; set; }
}