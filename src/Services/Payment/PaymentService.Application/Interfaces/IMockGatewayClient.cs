using PaymentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.Interfaces
{
    public interface IMockGatewayClient
    {
        Task<ChargeResultDto> ChargeAsync(ChargeRequestDto dto);
    }
}
