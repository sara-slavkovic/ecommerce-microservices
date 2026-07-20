using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get order by ID")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpGet("user/{userId:guid}")]
        [SwaggerOperation(Summary = "Get orders by user ID")]
        public async Task<IActionResult> GetOrdersByUserId(Guid userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create order from cart")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var order = await _orderService.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpPost("{id:guid}/complete")]
        [SwaggerOperation(Summary = "Complete order after successful payment")]
        public async Task<IActionResult> CompleteOrder(Guid id)
        {
            await _orderService.CompleteOrderAsync(id);
            return NoContent();
        }

        [HttpPost("{id:guid}/cancel")]
        [SwaggerOperation(Summary = "Cancel order after failed payment")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            await _orderService.CancelOrderAsync(id);
            return NoContent();
        }
    }
}
