using PaymentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Interfaces
{
    public interface IOrderServiceClient
    {
        Task<OrderSnapshotDto?> GetOrderByIdAsync(Guid orderId);
        Task CompleteOrderAsync(Guid orderId);
        Task CancelOrderAsync(Guid orderId);
    }
}
