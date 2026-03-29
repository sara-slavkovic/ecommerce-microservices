using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Application.DTOs
{
    public class CreateInventoryItemDto
    {
        public Guid ProductId { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
