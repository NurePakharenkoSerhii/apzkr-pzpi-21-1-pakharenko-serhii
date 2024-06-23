using API.Core.Contexts;
using API.Core.DTOs.Order;
using API.Core.Entities;
using API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class OrderService(GuardicDbContext context) : IOrderService
{
    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        return await context.Orders
            .Select(m => new OrderDto
            {
                ThingId = m.ThingId,
                Frequency = m.Frequency,
                StartTimeUTC = m.StartTimeUTC,
                EndTimeUTC = m.EndTimeUTC,
                AssignmentId = m.AssignmentId
            })
            .ToListAsync();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int thingId)
    {
        return await context.Orders
            .Where(m => m.ThingId == thingId)
            .Select(m => new OrderDto
            {
                ThingId = m.ThingId,
                Frequency = m.Frequency,
                StartTimeUTC = m.StartTimeUTC,
                EndTimeUTC = m.EndTimeUTC,
                AssignmentId = m.AssignmentId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> AddOrderAsync(OrderCreateDto orderDto)
    {
        var order = new Order
        {
            ThingId = orderDto.ThingId,
            Frequency = orderDto.Frequency,
            StartTimeUTC = orderDto.StartTimeUTC,
            EndTimeUTC = orderDto.EndTimeUTC,
            AssignmentId = orderDto.AssignmentId
        };

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        return order.ThingId;
    }

    public async Task<bool> UpdateOrderAsync(int thingId, OrderUpdateDto updatedOrderDto)
    {
        var existingOrder = await context.Orders.FindAsync(thingId);

        if (existingOrder == null)
            return false;

        existingOrder.Frequency = updatedOrderDto.Frequency;
        existingOrder.StartTimeUTC = updatedOrderDto.StartTimeUTC;
        existingOrder.EndTimeUTC = updatedOrderDto.EndTimeUTC;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteOrderAsync(int thingId)
    {
        var order = await context.Orders.FindAsync(thingId);

        if (order == null)
            return false;

        context.Orders.Remove(order);
        await context.SaveChangesAsync();
        return true;
    }
}
