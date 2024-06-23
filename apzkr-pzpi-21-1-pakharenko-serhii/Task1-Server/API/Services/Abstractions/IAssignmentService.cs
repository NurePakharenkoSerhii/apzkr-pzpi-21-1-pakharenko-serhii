using API.Core.DTOs.Assignment;

namespace API.Services.Abstractions;

public interface IAssignmentService
{
    Task<List<AssignmentDto>> GetUserAssignmentsAsync(int userId);
    Task<AssignmentDto?> GetAssignmentByIdAsync(int assignmentId);
    Task<int> AddAssignmentAsync(int userId, AssignmentCreateDto assignmentDto);
    Task<bool> UpdateAssignmentAsync(int assignmentId, AssignmentUpdateDto updatedAssignmentDto);
    Task<bool> DeleteAssignmentAsync(int assignmentId);
    Task VerifyAssignment(int guardId, VerifyAssignmentDto verifyAssignmentDto);
}