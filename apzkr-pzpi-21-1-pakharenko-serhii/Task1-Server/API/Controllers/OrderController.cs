using API.Core.Attributes;
using API.Core.DTOs.Order;
using API.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/order")]
[GuardRoleInterceptor]
[Authorize]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders()
    {
        var orders = await orderService.GetOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{thingId:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int thingId)
    {
        var order = await orderService.GetOrderByIdAsync(thingId);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddOrder([FromBody] OrderCreateDto orderDto)
    {
        var addedOrderId = await orderService.AddOrderAsync(orderDto);
        return Ok(addedOrderId);
    }

    [HttpPut("{thingId:int}")]
    public async Task<ActionResult<bool>> UpdateOrder(int thingId, [FromBody] OrderUpdateDto updatedOrderDto)
    {
        var result = await orderService.UpdateOrderAsync(thingId, updatedOrderDto);

        if (!result)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{thingId:int}")]
    public async Task<ActionResult<bool>> DeleteOrder(int thingId)
    {
        var result = await orderService.DeleteOrderAsync(thingId);

        if (!result)
            return NotFound();

        return Ok(result);
    }
}
