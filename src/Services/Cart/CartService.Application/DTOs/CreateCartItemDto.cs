using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Application.DTOs
{
    public class CreateCartItemDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
