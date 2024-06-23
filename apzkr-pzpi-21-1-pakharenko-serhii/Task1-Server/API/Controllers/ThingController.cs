using API.Core.DTOs.Thing;
using API.Core.DTOs.Assignment;
using API.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ThingController(IThingService thingService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ThingDto>>> GetThings()
    {
        var things = await thingService.GetThingsAsync();
        return Ok(things);
    }

    [HttpGet("{thingId:int}")]
    public async Task<ActionResult<ThingDto>> GetThingById(int thingId)
    {
        var thing = await thingService.GetThingByIdAsync(thingId);

        if (thing == null)
            return NotFound();

        return Ok(thing);
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddThing([FromBody] ThingCreateDto thing)
    {
        var addedThingId = await thingService.AddThingAsync(thing);
        return Ok(addedThingId);
    }

    [HttpPut("{thingId:int}")]
    public async Task<ActionResult<bool>> UpdateThing(int thingId, [FromBody] ThingUpdateDto updatedThing)
    {
        var result = await thingService.UpdateThingAsync(thingId, updatedThing);

        if (!result)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{thingId:int}")]
    public async Task<ActionResult<bool>> DeleteThing(int thingId)
    {
        var result = await thingService.DeleteThingAsync(thingId);

        if (!result)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("takeThings/{userId:int}")]
    public async Task<ActionResult<AssignedThingDtoList>> RetrieveUserThing(int userId)
    {
        List<AssignedThingDto> result;

        try
        {
            result = await thingService.GetAssignedThings(userId);
        }
        catch (ArgumentException)
        {
            // error in case of bad input
            return BadRequest("Bad input");
        }
        catch (FieldAccessException)
        {
            // error in case of no orders left to take today
            return BadRequest("No orders left to take today");
        }
        catch (Exception)
        {
            // error in case of unexpected error
            return Problem("Unexpected error happened.");
        }

        if (result.Count < 1)
        {
           // not found in case if no orders to take were found
            return NotFound("No prescribed orders were found. ");
        }
        
        return Ok(new AssignedThingDtoList(result));
    }
}
