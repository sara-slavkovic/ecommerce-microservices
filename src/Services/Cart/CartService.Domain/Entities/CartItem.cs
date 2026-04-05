using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Domain.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }

        public Cart Cart { get; set; } = null!;
    }
}
