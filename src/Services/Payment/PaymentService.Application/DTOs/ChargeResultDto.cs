using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.DTOs
{
    // response from mock payment gateway
    public class ChargeResultDto
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
    }
}
