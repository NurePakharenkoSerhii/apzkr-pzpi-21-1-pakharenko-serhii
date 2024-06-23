namespace API.Core.DTOs.Assignment;

public record AssignedThingDto(int AssignmentId, int OrderId, int Amount, int Frequency, int Duration, DateTime DateAssignedUTC);

public record AssignedThingDtoList(List<AssignedThingDto> AssignedThingDtos);
