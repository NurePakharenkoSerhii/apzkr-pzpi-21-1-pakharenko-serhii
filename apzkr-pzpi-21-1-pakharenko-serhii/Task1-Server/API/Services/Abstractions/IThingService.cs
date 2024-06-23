using API.Core.DTOs.Thing;
using API.Core.DTOs.Assignment;

namespace API.Services.Abstractions;

public interface IThingService
{
    Task<List<ThingDto>> GetThingsAsync();
    Task<ThingDto?> GetThingByIdAsync(int thingId);
    Task<int> AddThingAsync(ThingCreateDto thingCreateDto);
    Task<bool> UpdateThingAsync(int thingId, ThingUpdateDto updatedThing);
    Task<bool> DeleteThingAsync(int thingId);
    Task<List<AssignedThingDto>> GetAssignedThings(int userId);
}