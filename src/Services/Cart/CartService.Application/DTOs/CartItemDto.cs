using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Application.DTOs
{
    public class CartItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
