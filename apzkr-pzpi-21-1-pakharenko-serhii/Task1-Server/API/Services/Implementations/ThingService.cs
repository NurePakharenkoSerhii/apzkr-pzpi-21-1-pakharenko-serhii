using API.Core.Contexts;
using API.Core.DTOs.Thing;
using API.Core.DTOs.Assignment;
using API.Core.Entities;
using API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class ThingService(GuardicDbContext context) : IThingService
{
    public async Task<List<ThingDto>> GetThingsAsync()
    {
        return await context.Things.Select(m => new ThingDto
        {
            ThingId = m.Id,
            Name = m.Name,
            Description = m.Description,
            Effect = m.Effect,
            Interactions = m.Interactions
        }).ToListAsync();
    }

    public async Task<ThingDto?> GetThingByIdAsync(int thingId)
    {
        return await context.Things.Where(m => m.Id == thingId)
            .Select(m => new ThingDto
            {
                ThingId = m.Id,
                Name = m.Name,
                Description = m.Description,
                Effect = m.Effect,
                Interactions = m.Interactions
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> AddThingAsync(ThingCreateDto thingCreateDto)
    {
        var thing = new Thing
        {
            Name = thingCreateDto.Name,
            Description = thingCreateDto.Description,
            Effect = thingCreateDto.Effect,
            Interactions = thingCreateDto.Interactions
        };
        
        await context.Things.AddAsync(thing);
        await context.SaveChangesAsync();
        return thing.Id;
    }

    public async Task<bool> UpdateThingAsync(int thingId, ThingUpdateDto updatedThing)
    {
        var existingThing = await context.Things.FindAsync(thingId);

        if (existingThing == null)
            return false;

        existingThing.Name = updatedThing.Name;
        existingThing.Description = updatedThing.Description;
        existingThing.Effect = updatedThing.Effect;
        existingThing.Interactions = updatedThing.Interactions;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteThingAsync(int thingId)
    {
        var thing = await context.Things.FindAsync(thingId);

        if (thing == null)
            return false;

        context.Things.Remove(thing);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<AssignedThingDto>> GetAssignedThings(int userId)
    {
        var currentDateUTC = DateTime.UtcNow.Date;
        
        var prescriptedThings = (await context.Assignments
                .Include(x => x.Orders)
                .Where(p => p.UserId == userId && p.DateAssignedUTC != null)
                .ToListAsync())
            .SelectMany(p => p.Orders
                .Select(m => new AssignedThingDto(
                    p.Id,
                    m.ThingId,
                    p.Amount,
                    m.Frequency,
                    p.Duration,
                    p.DateAssignedUTC!.Value
                )))
            .Where(dto => dto.Duration > 0 && dto.DateAssignedUTC.AddDays(dto.Duration) >= currentDateUTC)
            .Where(dto => dto.Frequency > 0) // Frequency validation
            .ToList();

        var resPrescripterThings = new List<AssignedThingDto>(prescriptedThings);
        foreach (var orderDto in prescriptedThings)
        {
            var thing = await context.Things.FirstOrDefaultAsync(m => m.Id == orderDto.OrderId);
            if (thing?.ExpirationDate < DateTime.Now)
            {
                var guardsWithThingAsAssignments = await context.Guards.Include(d => d.Assignments)
                    .Where(d => d.Assignments != null).ToListAsync();

                foreach (var guard in guardsWithThingAsAssignments)
                {
                    var notification = new Notification()
                    {
                        GuardId = guard.Id,
                        isRead = false,
                        Message = $"The thing ${thing.Name} with the number ${thing.Id} has expired. Replace the thing or change the assignment.",
                    };

                    context.Notifications.Add(notification);
                }
            }
                
            var orderLogsCount = context.OrderLogs
                .Count(log => log.OrderId == orderDto.OrderId
                              && log.UserId == userId
                              && log.TimestampUTC.Date == currentDateUTC.Date);

            if (orderLogsCount >= orderDto.Frequency)
            {
                // ignore in case the order was already taken for maximum frequency
                resPrescripterThings.Remove(orderDto);
                continue;
            }
            
            // Log order taken
            var logEntry = new OrderLog
            {
                UserId = userId,
                OrderId = orderDto.OrderId,
                TimestampUTC = DateTime.UtcNow,
                Status = "Taken"
            };

            context.OrderLogs.Add(logEntry);
        }

        if (prescriptedThings.Count > 0 && resPrescripterThings.Count < 1)
        {
            // error in case orders was already taken for maximum frequency
            throw new FieldAccessException();
        }

        await context.SaveChangesAsync();

        return resPrescripterThings;
    }
}
