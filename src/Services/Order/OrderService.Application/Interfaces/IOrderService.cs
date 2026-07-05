using OrderService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto?> GetOrderByIdAsync(Guid id);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(Guid userId);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
        Task CompleteOrderAsync(Guid orderId);
        Task CancelOrderAsync(Guid orderId);
    }
}
