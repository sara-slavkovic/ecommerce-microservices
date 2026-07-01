using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Application.DTOs
{
    public class InventoryAvailabilityDto
    {
        public Guid ProductId { get; set; }
        public int AvailableQuantity { get; set; }
        public int ReservedQuantity { get; set; }
    }
}
