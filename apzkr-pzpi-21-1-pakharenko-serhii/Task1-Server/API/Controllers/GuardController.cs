using API.Core.Constants;
using API.Core.DTOs.Guard;
using API.Core.Entities;
using API.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GuardController(IGuardService guardService, IJwtService jwtService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Guard>>> GetGuards()
    {
        var guards = await guardService.GetGuardsAsync();
        return Ok(guards);
    }
    
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<Guard>> GetGuard(int id)
    {
        var guard = await guardService.GetGuardAsync(id);
        if (guard == null)
        {
            return NotFound();
        }
        return Ok(guard);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guard>> CreateGuard([FromBody] CreateGuardDto guard)
    {
        try
        {
            await guardService.CreateGuardAsync(guard);
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
    
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateGuard(int id, [FromBody] UpdateGuardDto guard)
    {
        try
        {
            await guardService.UpdateGuardAsync(id, guard);
            return NoContent();
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
    
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] GuardLoginDto guardLoginDto)
    {
        var guard = await guardService.LogInAsync(guardLoginDto.Email, guardLoginDto.Password);

        if (guard == null)
        {
            return Unauthorized("Invalid email or password");
        }
        
        var token = jwtService.GenerateToken(guard.Id, Role.GuardRole);
        return Ok(token);
    }
    
    [HttpPost("signup")]
    public async Task<ActionResult> SignUp([FromBody] GuardSignUpDto guardSignUpDto)
    {
        try
        {
            var result = await guardService.SignUpAsync(guardSignUpDto);

            if (result)
            {
                return Ok("Guard registration successful");
            }

            return BadRequest("Guard with the same email already exists");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}
