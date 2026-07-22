using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.DTOs
{
    public class OrderSnapshotDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
