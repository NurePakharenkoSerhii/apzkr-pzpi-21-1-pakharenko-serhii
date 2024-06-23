using System.Security.Claims;
using API.Core.Attributes;
using API.Core.DTOs.Assignment;
using API.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/assignment")]
[Authorize]
public class AssignmentController(IAssignmentService assignmentService) : ControllerBase
{
    [HttpGet("my")]
    public async Task<ActionResult<List<AssignmentDto>>> GetMyAssignments()
    {
        var userId = Convert.ToInt32((HttpContext.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value);
        var assignments = await assignmentService.GetUserAssignmentsAsync(userId);
        return Ok(assignments);
    }

    [HttpGet("{assignmentId:int}")]
    public async Task<ActionResult<AssignmentDto>> GetAssignmentById(int assignmentId)
    {
        var assignment = await assignmentService.GetAssignmentByIdAsync(assignmentId);

        if (assignment == null)
            return NotFound();

        return Ok(assignment);
    }
    
    [HttpPost]
    public async Task<ActionResult<int>> AddAssignment([FromBody] AssignmentCreateDto assignmentDto)
    {
        var userId = Convert.ToInt32((HttpContext.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value);
        var addedAssignmentId = await assignmentService.AddAssignmentAsync(userId, assignmentDto);
        return Ok(addedAssignmentId);
    }

    [HttpPut("{assignmentId:int}")]
    [GuardRoleInterceptor]
    public async Task<ActionResult<bool>> UpdateAssignment(int assignmentId, [FromBody] AssignmentUpdateDto updatedAssignmentDto)
    {
        var result = await assignmentService.UpdateAssignmentAsync(assignmentId, updatedAssignmentDto);

        if (!result)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{assignmentId:int}")]
    [GuardRoleInterceptor]
    public async Task<ActionResult<bool>> DeleteAssignment(int assignmentId)
    {
        var result = await assignmentService.DeleteAssignmentAsync(assignmentId);

        if (!result)
            return NotFound();

        return Ok(result);
    }
    
    [HttpPost("verify")]
    [GuardRoleInterceptor]
    public async Task<ActionResult<bool>> VerifyAssignment(VerifyAssignmentDto verifyAssignmentDto)
    {
        try
        {
            var guardId = Convert.ToInt32((HttpContext.User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value);
            await assignmentService.VerifyAssignment(guardId, verifyAssignmentDto);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }
    }
}
