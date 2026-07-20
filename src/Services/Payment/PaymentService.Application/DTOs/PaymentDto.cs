using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.DTOs
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PaymentAttemptDto> PaymentAttempts { get; set; } = new();
    }
}
