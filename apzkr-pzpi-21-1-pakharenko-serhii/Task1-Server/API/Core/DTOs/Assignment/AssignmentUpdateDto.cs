namespace API.Core.DTOs.Assignment;

public record AssignmentUpdateDto
{
    public int Amount { get; set; }
    public int Duration { get; set; }
}