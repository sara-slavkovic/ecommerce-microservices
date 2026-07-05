using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.DTOs
{
    public class CartSnapshotDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CartItemSnapshotDto> CartItems { get; set; } = new();
    }
}
