using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Application.DTOs
{
    public class UpdateInventoryItemDto
    {
        public int AvailableQuantity { get; set; }
        public int ReservedQuantity { get; set; }
    }
}
