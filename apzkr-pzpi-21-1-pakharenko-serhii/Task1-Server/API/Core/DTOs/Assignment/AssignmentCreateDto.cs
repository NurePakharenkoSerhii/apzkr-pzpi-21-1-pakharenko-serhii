using API.Core.DTOs.Thing;

namespace API.Core.DTOs.Assignment;

public record AssignmentCreateDto
{
    public int Amount { get; set; }
    public int Duration { get; set; }
    public List<AddAssignmentThingDto> Things { get; set; }
}