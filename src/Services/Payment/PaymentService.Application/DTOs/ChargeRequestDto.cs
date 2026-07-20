using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.DTOs
{
    // request to mock payment gateway
    public class ChargeRequestDto
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string IdempotencyKey { get; set; } = string.Empty;
        public string SimulationMode { get; set; } = "Random";
        public int FailCount { get; set; } = 0;
    }
}
