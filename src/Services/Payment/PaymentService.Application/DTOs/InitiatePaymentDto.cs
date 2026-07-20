using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Application.DTOs
{
    //initiation of payment - when user clicks "pay"
    public class InitiatePaymentDto
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}
