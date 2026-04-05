using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Application.Enums
{
    public enum ReserveInventoryResult
    {
        Success,
        InventoryItemNotFound,
        InvalidQuantity,
        InsufficientAvailableQuantity
    }
}
