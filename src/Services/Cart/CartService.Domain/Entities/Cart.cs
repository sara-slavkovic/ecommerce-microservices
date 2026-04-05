using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
