using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Domain.Entities
{
    public class InventoryItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int AvailableQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
