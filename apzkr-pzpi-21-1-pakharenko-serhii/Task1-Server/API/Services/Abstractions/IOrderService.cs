using API.Core.DTOs.Order;

namespace API.Services.Abstractions;

public interface IOrderService
{
    Task<List<OrderDto>> GetOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(int thingId);
    Task<int> AddOrderAsync(OrderCreateDto orderDto);
    Task<bool> UpdateOrderAsync(int thingId, OrderUpdateDto updatedOrderDto);
    Task<bool> DeleteOrderAsync(int thingId);
}
