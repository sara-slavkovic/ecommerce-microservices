using PaymentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetPaymentByIdAsync(Guid id);
        Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId);
        Task AddPaymentAsync(Payment payment);
        Task SaveChangesAsync();
    }
}
