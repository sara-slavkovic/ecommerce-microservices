using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Application.DTOs
{
    public class ChangeInventoryQuantityDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
