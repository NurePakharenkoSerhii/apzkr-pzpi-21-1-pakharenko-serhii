namespace API.Core.DTOs.Assignment;

public record VerifyAssignmentDto(int assignmentId, int amount, int duration, DateTime endTimeUtc);