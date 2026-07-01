using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Application.DTOs
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<CartItemDto> CartItems { get; set; } = new();
    }
}
