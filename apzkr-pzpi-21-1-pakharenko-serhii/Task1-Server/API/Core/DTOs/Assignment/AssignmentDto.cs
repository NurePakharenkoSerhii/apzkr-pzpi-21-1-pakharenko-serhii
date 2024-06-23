using API.Core.DTOs.Order;

namespace API.Core.DTOs.Assignment;

public record AssignmentDto
{
    public int Id { get; set; }
    public DateTime? DateAssignedUTC { get; set; }
    public int UserId { get; set; }
    public int? GuardId { get; set; }
    public int Amount { get; set; }
    public int Duration { get; set; }
    public List<AssignmentOrderDto> Orders { get; set; }
}