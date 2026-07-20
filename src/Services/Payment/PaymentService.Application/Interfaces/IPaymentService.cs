using PaymentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDto?> GetPaymentByIdAsync(Guid id);
        Task<PaymentDto?> GetPaymentByOrderIdAsync(Guid orderId);
        Task<PaymentDto> InitiatePaymentAsync(InitiatePaymentDto dto);
    }
}
