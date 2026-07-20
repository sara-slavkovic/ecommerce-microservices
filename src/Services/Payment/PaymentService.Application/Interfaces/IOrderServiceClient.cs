using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Interfaces
{
    public interface IOrderServiceClient
    {
        Task CompleteOrderAsync(Guid orderId);
        Task CancelOrderAsync(Guid orderId);
    }
}
