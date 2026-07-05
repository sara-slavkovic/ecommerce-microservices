using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.DTOs
{
    public class InventoryChangeDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
