using API.Core.Contexts;
using API.Core.DTOs.Order;
using API.Core.DTOs.Thing;
using API.Core.DTOs.Assignment;
using API.Core.Entities;
using API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class AssignmentService(GuardicDbContext context): IAssignmentService
{
    public async Task<List<AssignmentDto>> GetUserAssignmentsAsync(int userId)
    {
        var assignments = await context.Assignments
            .Include(x => x.Orders)
            .Where(x => x.UserId == userId)
            .Select(p => new AssignmentDto
            {
                Id = p.Id,
                DateAssignedUTC = p.DateAssignedUTC,
                UserId = p.UserId,
                GuardId = p.GuardId,
                Amount = p.Amount,
                Duration = p.Duration,
                Orders = context.Orders
                    .Include(m => m.Thing)
                    .Where(x => x.AssignmentId == p.Id)
                    .Select(x => new AssignmentOrderDto
                    {
                        Thing = new ThingDto
                        {
                            ThingId = x.ThingId,
                            Name = x.Thing.Name,
                            Description = x.Thing.Description,
                            Effect = x.Thing.Effect,
                            Interactions = x.Thing.Interactions
                        },
                        Frequency = x.Frequency,
                        StartTimeUTC = x.StartTimeUTC,
                        EndTimeUTC = x.EndTimeUTC
                    })
                    .ToList()
            })
            .ToListAsync();

        return assignments;
    }

    public async Task<AssignmentDto?> GetAssignmentByIdAsync(int assignmentId)
    {
        return await context.Assignments
            .Where(p => p.Id == assignmentId)
            .Select(p => new AssignmentDto
            {
                Id = p.Id,
                DateAssignedUTC = p.DateAssignedUTC,
                UserId = p.UserId,
                GuardId = p.GuardId,
                Amount = p.Amount,
                Duration = p.Duration,
                Orders = context.Orders
                    .Include(m => m.Thing)
                    .Select(x => new AssignmentOrderDto
                    {
                        Thing = new ThingDto
                        {
                            ThingId = x.ThingId,
                            Name = x.Thing.Name,
                            Description = x.Thing.Description,
                            Effect = x.Thing.Effect,
                            Interactions = x.Thing.Interactions
                        },
                        Frequency = x.Frequency,
                        StartTimeUTC = x.StartTimeUTC,
                        EndTimeUTC = x.EndTimeUTC
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> AddAssignmentAsync(int userId, AssignmentCreateDto assignmentDto)
    {
        foreach (var thing in assignmentDto.Things)
        {
            await context.Things.FirstAsync(x => x.Id == thing.ThingId); // throws an exception if thing doesn't exist
        }
        
        var assignment = new Assignment
        {
            UserId = userId,
            Amount = assignmentDto.Amount,
            Duration = assignmentDto.Duration
        };

        await context.Assignments.AddAsync(assignment);
        await context.SaveChangesAsync();

        foreach (var thing in assignmentDto.Things)
        {
            var order = new Order
            {
                AssignmentId = assignment.Id,
                ThingId = thing.ThingId,
                Frequency = thing.Frequency
            };

            await context.Orders.AddAsync(order);
        }
        
        await context.SaveChangesAsync();

        return assignment.Id;
    }

    public async Task<bool> UpdateAssignmentAsync(int assignmentId, AssignmentUpdateDto updatedAssignmentDto)
    {
        var existingAssignment = await context.Assignments.FindAsync(assignmentId);

        if (existingAssignment == null)
            return false;

        existingAssignment.Amount = updatedAssignmentDto.Amount;
        existingAssignment.Duration = updatedAssignmentDto.Duration;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAssignmentAsync(int assignmentId)
    {
        var assignment = await context.Assignments.FindAsync(assignmentId);

        if (assignment == null)
            return false;

        context.Assignments.Remove(assignment);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task VerifyAssignment(int guardId, VerifyAssignmentDto verifyAssignmentDto)
    {
        var assignment = await context.Assignments
            .Include(assignment => assignment.Guard)
            .FirstOrDefaultAsync(p => p.Id == verifyAssignmentDto.assignmentId);

        if (assignment is not {Guard: null})
        {
            throw new ArgumentException(nameof(assignment));
        }

        if (verifyAssignmentDto.amount <= 0)
        {
            throw new ArgumentException(nameof(verifyAssignmentDto.amount));
        }
        
        if (verifyAssignmentDto.duration <= 0)
        {
            throw new ArgumentException(nameof(verifyAssignmentDto.duration));
        }

        var curDateTimeUtc = DateTime.UtcNow;
        
        assignment.GuardId = guardId;
        assignment.Amount = verifyAssignmentDto.amount;
        assignment.Duration = verifyAssignmentDto.duration;
        assignment.DateAssignedUTC = curDateTimeUtc;

        context.Assignments.Update(assignment);

        foreach (var order in context.Orders.Where(x => x.AssignmentId == verifyAssignmentDto.assignmentId))
        {
            order.StartTimeUTC = curDateTimeUtc;
            order.EndTimeUTC = verifyAssignmentDto.endTimeUtc;
            context.Orders.Update(order);
        }
        
        await context.SaveChangesAsync();
    }
}
