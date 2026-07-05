using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Domain.Enums
{
    public enum OrderStatus
    {
        Created,
        Pending,
        Paid,
        Cancelled
    }
}
