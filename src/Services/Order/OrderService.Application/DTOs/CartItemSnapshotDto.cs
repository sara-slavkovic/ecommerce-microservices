using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.DTOs
{
    public class CartItemSnapshotDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
    }
}
